using AKLC.Application.DTOs.Students;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // GET ALL STUDENTS
        // READ ONLY
        // =========================================

        public async Task<IReadOnlyList<Student>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .Include(student => student.Course)
                .Include(student => student.Batch)
                .Where(student =>
                    !student.IsDeleted)
                .OrderByDescending(student =>
                    student.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET STUDENT BY ID
        // READ ONLY
        // =========================================

        public async Task<Student?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .Include(student => student.Course)
                .Include(student => student.Batch)
                .FirstOrDefaultAsync(
                    student =>
                        student.Id == id &&
                        !student.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // GET STUDENT FOR UPDATE
        // TRACKED ENTITY
        // =========================================

        public async Task<Student?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .FirstOrDefaultAsync(
                    student =>
                        student.Id == id &&
                        !student.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // ADD STUDENT
        // =========================================

        public async Task AddAsync(
            Student student,
            CancellationToken cancellationToken = default)
        {
            await _context.Students
                .AddAsync(
                    student,
                    cancellationToken);
        }


        // =========================================
        // EXISTS BY STUDENT CODE
        // =========================================

        public async Task<bool> ExistsByStudentCodeAsync(
            string studentCode,
            CancellationToken cancellationToken = default)
        {
            return await _context.Students
                .AsNoTracking()
                .AnyAsync(
                    student =>
                        student.StudentCode == studentCode,
                    cancellationToken);
        }


        // =========================================
        // SAVE CHANGES
        // =========================================

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }


        // =========================================
        // GET STUDENT SUMMARY
        // =========================================

        public async Task<StudentSummaryDto> GetSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // BASIC STUDENT COUNTS
            // -------------------------------------

            var totalStudents =
                await _context.Students
                    .AsNoTracking()
                    .CountAsync(
                        student =>
                            !student.IsDeleted,
                        cancellationToken);


            var activeStudents =
                await _context.Students
                    .AsNoTracking()
                    .CountAsync(
                        student =>
                            !student.IsDeleted &&
                            student.IsActive,
                        cancellationToken);


            // -------------------------------------
            // TOTAL PAYABLE BY STUDENT
            // ACTIVE + NON-DELETED ASSIGNMENTS
            // -------------------------------------

            var payableByStudent =
                await _context.StudentFeeAssignments
                    .AsNoTracking()
                    .Where(assignment =>
                        assignment.IsActive &&
                        !assignment.IsDeleted &&
                        !assignment.Student.IsDeleted &&
                        assignment.Student.IsActive)
                    .GroupBy(assignment =>
                        assignment.StudentId)
                    .Select(group => new
                    {
                        StudentId =
                            group.Key,

                        TotalPayable =
                            group.Sum(x =>
                                x.NetAmount)
                    })
                    .ToListAsync(
                        cancellationToken);


            // -------------------------------------
            // TOTAL VALID PAYMENTS BY STUDENT
            // REVERSED PAYMENTS EXCLUDED
            // -------------------------------------

            var paidByStudent =
                await _context.StudentPayments
                    .AsNoTracking()
                    .Where(payment =>
                        !payment.IsReversed &&
                        !payment.Student.IsDeleted &&
                        payment.Student.IsActive)
                    .GroupBy(payment =>
                        payment.StudentId)
                    .Select(group => new
                    {
                        StudentId =
                            group.Key,

                        TotalPaid =
                            group.Sum(x =>
                                x.Amount)
                    })
                    .ToDictionaryAsync(
                        x => x.StudentId,
                        x => x.TotalPaid,
                        cancellationToken);


            // -------------------------------------
            // CALCULATE DUE STUDENTS
            // -------------------------------------

            var dueStudentCount = 0;

            decimal totalOutstanding = 0m;


            foreach (var payable in payableByStudent)
            {
                paidByStudent.TryGetValue(
                    payable.StudentId,
                    out var totalPaid);


                var outstanding =
                    payable.TotalPayable -
                    totalPaid;


                if (outstanding > 0)
                {
                    dueStudentCount++;

                    totalOutstanding +=
                        outstanding;
                }
            }


            // -------------------------------------
            // RETURN SUMMARY
            // -------------------------------------

            return new StudentSummaryDto
            {
                TotalStudents =
                    totalStudents,

                ActiveStudents =
                    activeStudents,

                DueStudentCount =
                    dueStudentCount,

                TotalOutstanding =
                    totalOutstanding
            };
        }


        // =========================================
        // GET STUDENTS DUE ON SPECIFIC DATE
        //
        // BUSINESS RULE:
        //
        // 1. If a fee assignment has active
        //    payment schedules, schedule DueDate
        //    controls payment timing.
        //
        // 2. If a fee assignment has NO active
        //    payment schedules, assignment DueDate
        //    is used as fallback.
        //
        // 3. Assignment and schedule are never
        //    counted as separate liabilities.
        // =========================================

        public async Task<IReadOnlyList<TodayDueStudentDto>>
            GetDueStudentsByDateAsync(
                DateOnly date,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // LOAD ACTIVE PAYMENT SCHEDULES
            //
            // Future schedules are also loaded
            // because they are required for
            // NextPaymentDate calculation.
            // =====================================

            var schedules =
                await _context.PaymentSchedules
                    .AsNoTracking()
                    .Include(schedule =>
                        schedule.Student)
                    .Include(schedule =>
                        schedule.Allocations)
                        .ThenInclude(allocation =>
                            allocation.StudentPayment)
                    .Where(schedule =>
                        schedule.IsActive &&
                        !schedule.IsDeleted &&
                        !schedule.Student.IsDeleted &&
                        schedule.Student.IsActive)
                    .OrderBy(schedule =>
                        schedule.DueDate)
                    .ThenBy(schedule =>
                        schedule.CreatedAt)
                    .ToListAsync(
                        cancellationToken);


            // =====================================
            // CALCULATE ALL OPEN SCHEDULES
            //
            // Allocations belonging to reversed
            // payments do not count as paid.
            // =====================================

            var openSchedules =
                schedules
                    .Select(schedule =>
                    {
                        var validPaidAmount =
                            schedule.Allocations
                                .Where(allocation =>
                                    allocation.StudentPayment != null &&
                                    !allocation
                                        .StudentPayment
                                        .IsReversed)
                                .Sum(allocation =>
                                    allocation.AllocatedAmount);


                        var remainingAmount =
                            Math.Max(
                                0m,
                                schedule.ScheduledAmount -
                                validPaidAmount);


                        return new
                        {
                            Schedule =
                                schedule,

                            RemainingAmount =
                                remainingAmount
                        };
                    })
                    .Where(item =>
                        item.RemainingAmount > 0)
                    .ToList();


            // =====================================
            // LOAD ACTIVE FEE ASSIGNMENTS
            //
            // DueDate from an assignment is only
            // a fallback when that assignment has
            // no active/non-deleted schedules.
            // =====================================

            var assignments =
                await _context.StudentFeeAssignments
                    .AsNoTracking()
                    .Include(assignment =>
                        assignment.Student)
                    .Include(assignment =>
                        assignment.PaymentSchedules)
                    .Where(assignment =>
                        assignment.IsActive &&
                        !assignment.IsDeleted &&
                        assignment.DueDate.HasValue &&
                        !assignment.Student.IsDeleted &&
                        assignment.Student.IsActive)
                    .ToListAsync(
                        cancellationToken);


            // =====================================
            // ASSIGNMENTS WITHOUT ACTIVE SCHEDULE
            // =====================================

            var unscheduledAssignments =
                assignments
                    .Where(assignment =>
                        !assignment.PaymentSchedules.Any(
                            schedule =>
                                schedule.IsActive &&
                                !schedule.IsDeleted))
                    .ToList();


            // =====================================
            // STUDENTS REQUIRING FALLBACK LOGIC
            // =====================================

            var studentIds =
                unscheduledAssignments
                    .Select(assignment =>
                        assignment.StudentId)
                    .Distinct()
                    .ToList();


            // =====================================
            // VALID PAYMENTS BY STUDENT
            //
            // Reversed payments are excluded.
            // =====================================

            var validPaidByStudent =
                await _context.StudentPayments
                    .AsNoTracking()
                    .Where(payment =>
                        studentIds.Contains(
                            payment.StudentId) &&
                        !payment.IsReversed)
                    .GroupBy(payment =>
                        payment.StudentId)
                    .Select(group => new
                    {
                        StudentId =
                            group.Key,

                        TotalPaid =
                            group.Sum(payment =>
                                payment.Amount)
                    })
                    .ToDictionaryAsync(
                        item =>
                            item.StudentId,
                        item =>
                            item.TotalPaid,
                        cancellationToken);


            // =====================================
            // TOTAL ACTIVE LIABILITY BY STUDENT
            // =====================================

            var totalPayableByStudent =
                await _context.StudentFeeAssignments
                    .AsNoTracking()
                    .Where(assignment =>
                        studentIds.Contains(
                            assignment.StudentId) &&
                        assignment.IsActive &&
                        !assignment.IsDeleted)
                    .GroupBy(assignment =>
                        assignment.StudentId)
                    .Select(group => new
                    {
                        StudentId =
                            group.Key,

                        TotalPayable =
                            group.Sum(assignment =>
                                assignment.NetAmount)
                    })
                    .ToDictionaryAsync(
                        item =>
                            item.StudentId,
                        item =>
                            item.TotalPayable,
                        cancellationToken);


            // =====================================
            // RESULT COLLECTION
            //
            // Dictionary prevents duplicate
            // student rows when multiple fees are
            // due on the same date.
            // =====================================

            var result =
                new Dictionary<Guid, TodayDueStudentDto>();


            // =====================================
            // 1. SCHEDULE-BASED DUE TODAY
            // =====================================

            var scheduleGroups =
                openSchedules
                    .Where(item =>
                        item.Schedule.DueDate ==
                        date)
                    .GroupBy(item =>
                        item.Schedule.StudentId);


            foreach (var group in scheduleGroups)
            {
                var student =
                    group.First()
                        .Schedule.Student;


                var dueAmount =
                    group.Sum(item =>
                        item.RemainingAmount);


                // ---------------------------------
                // NEXT FUTURE OPEN SCHEDULE
                // ---------------------------------

                var nextFutureSchedule =
                    openSchedules
                        .Where(item =>
                            item.Schedule.StudentId ==
                                group.Key &&
                            item.Schedule.DueDate >
                                date)
                        .OrderBy(item =>
                            item.Schedule.DueDate)
                        .ThenBy(item =>
                            item.Schedule.CreatedAt)
                        .FirstOrDefault();


                var nextPaymentDate =
                    nextFutureSchedule?
                        .Schedule.DueDate
                    ?? date;


                result[student.Id] =
                    new TodayDueStudentDto
                    {
                        Id =
                            student.Id,

                        StudentCode =
                            student.StudentCode,

                        FullName =
                            student.FullName,

                        MobileNumber =
                            student.MobileNumber,

                        DueAmount =
                            dueAmount,

                        NextPaymentDate =
                            nextPaymentDate
                    };
            }


            // =====================================
            // 2. ASSIGNMENT FALLBACK DUE TODAY
            //
            // Only assignments WITHOUT active
            // schedules are considered here.
            // =====================================

            var fallbackGroups =
                unscheduledAssignments
                    .Where(assignment =>
                        assignment.DueDate ==
                        date)
                    .GroupBy(assignment =>
                        assignment.StudentId);


            foreach (var group in fallbackGroups)
            {
                var student =
                    group.First()
                        .Student;


                // ---------------------------------
                // TOTAL PAYABLE
                // ---------------------------------

                totalPayableByStudent.TryGetValue(
                    student.Id,
                    out var totalPayable);


                // ---------------------------------
                // TOTAL VALID PAID
                // ---------------------------------

                validPaidByStudent.TryGetValue(
                    student.Id,
                    out var totalPaid);


                // ---------------------------------
                // OVERALL OUTSTANDING
                //
                // This prevents a fully paid or
                // advance-paid student from being
                // shown as due.
                // ---------------------------------

                var overallOutstanding =
                    Math.Max(
                        0m,
                        totalPayable -
                        totalPaid);


                if (overallOutstanding <= 0)
                {
                    continue;
                }


                // ---------------------------------
                // ASSIGNMENT AMOUNT DUE TODAY
                // ---------------------------------

                var assignmentDueAmount =
                    group.Sum(assignment =>
                        assignment.NetAmount);


                // Never show more than the actual
                // overall outstanding balance.

                var dueAmount =
                    Math.Min(
                        assignmentDueAmount,
                        overallOutstanding);


                if (dueAmount <= 0)
                {
                    continue;
                }


                // ---------------------------------
                // STUDENT MAY ALREADY EXIST
                //
                // Example:
                // One scheduled fee + another
                // unscheduled fee are both due
                // today.
                //
                // IMPORTANT:
                // Combined schedule + fallback due
                // must never exceed the student's
                // actual overall outstanding.
                // ---------------------------------

                if (result.TryGetValue(
                    student.Id,
                    out var existing))
                {
                    var remainingOutstanding =
                        Math.Max(
                            0m,
                            overallOutstanding -
                            existing.DueAmount);


                    var additionalDueAmount =
                        Math.Min(
                            dueAmount,
                            remainingOutstanding);


                    if (additionalDueAmount > 0)
                    {
                        existing.DueAmount +=
                            additionalDueAmount;
                    }

                    continue;
                }


                // ---------------------------------
                // NEXT FUTURE OPEN SCHEDULE
                // ---------------------------------

                var nextFutureScheduleDate =
                    openSchedules
                        .Where(item =>
                            item.Schedule.StudentId ==
                                student.Id &&
                            item.Schedule.DueDate >
                                date)
                        .OrderBy(item =>
                            item.Schedule.DueDate)
                        .ThenBy(item =>
                            item.Schedule.CreatedAt)
                        .Select(item =>
                            (DateOnly?)
                            item.Schedule.DueDate)
                        .FirstOrDefault();


                // ---------------------------------
                // NEXT FUTURE UNSCHEDULED
                // ASSIGNMENT
                // ---------------------------------

                var nextFutureAssignmentDate =
                    unscheduledAssignments
                        .Where(assignment =>
                            assignment.StudentId ==
                                student.Id &&
                            assignment.DueDate.HasValue &&
                            assignment.DueDate.Value >
                                date)
                        .OrderBy(assignment =>
                            assignment.DueDate)
                        .ThenBy(assignment =>
                            assignment.CreatedAt)
                        .Select(assignment =>
                            assignment.DueDate)
                        .FirstOrDefault();


                // ---------------------------------
                // DETERMINE NEXT PAYMENT DATE
                // ---------------------------------

                DateOnly nextPaymentDate =
                    date;


                if (nextFutureScheduleDate.HasValue &&
                    nextFutureAssignmentDate.HasValue)
                {
                    nextPaymentDate =
                        nextFutureScheduleDate.Value <=
                        nextFutureAssignmentDate.Value
                            ? nextFutureScheduleDate.Value
                            : nextFutureAssignmentDate.Value;
                }
                else if (nextFutureScheduleDate.HasValue)
                {
                    nextPaymentDate =
                        nextFutureScheduleDate.Value;
                }
                else if (nextFutureAssignmentDate.HasValue)
                {
                    nextPaymentDate =
                        nextFutureAssignmentDate.Value;
                }


                // ---------------------------------
                // ADD RESULT
                // ---------------------------------

                result[student.Id] =
                    new TodayDueStudentDto
                    {
                        Id =
                            student.Id,

                        StudentCode =
                            student.StudentCode,

                        FullName =
                            student.FullName,

                        MobileNumber =
                            student.MobileNumber,

                        DueAmount =
                            dueAmount,

                        NextPaymentDate =
                            nextPaymentDate
                    };
            }


            // =====================================
            // RETURN
            // =====================================

            return result.Values
                .OrderBy(student =>
                    student.FullName)
                .ToList();
        }
    }
}