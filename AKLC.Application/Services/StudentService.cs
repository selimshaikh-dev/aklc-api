using AKLC.Application.DTOs.Students;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        private readonly IStudentFeeAssignmentRepository
            _studentFeeAssignmentRepository;

        private readonly IStudentPaymentRepository
            _studentPaymentRepository;

        private readonly IPaymentScheduleRepository
            _paymentScheduleRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentService(
            IStudentRepository studentRepository,
            IStudentFeeAssignmentRepository studentFeeAssignmentRepository,
            IStudentPaymentRepository studentPaymentRepository,
            IPaymentScheduleRepository paymentScheduleRepository)
        {
            _studentRepository =
                studentRepository;

            _studentFeeAssignmentRepository =
                studentFeeAssignmentRepository;

            _studentPaymentRepository =
                studentPaymentRepository;

            _paymentScheduleRepository =
                paymentScheduleRepository;
        }


        // =========================================
        // GET ALL STUDENTS
        // =========================================

        public async Task<IReadOnlyList<StudentDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var students =
                await _studentRepository.GetAllAsync(
                    cancellationToken);

            return students
                .Select(MapToDto)
                .ToList();
        }


        // =========================================
        // GET STUDENT BY ID
        // =========================================

        public async Task<StudentDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var student =
                await _studentRepository.GetByIdAsync(
                    id,
                    cancellationToken);

            return student is null
                ? null
                : MapToDto(student);
        }


        // =========================================
        // CREATE STUDENT
        // =========================================

        public async Task<StudentDto> CreateAsync(
            CreateStudentRequest request,
            CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // REQUIRED VALIDATION
            // -------------------------------------

            if (string.IsNullOrWhiteSpace(
                request.FullName))
            {
                throw new ArgumentException(
                    "Student full name is required.");
            }

            if (string.IsNullOrWhiteSpace(
                request.MobileNumber))
            {
                throw new ArgumentException(
                    "Student mobile number is required.");
            }


            // -------------------------------------
            // GENERATE STUDENT CODE
            // -------------------------------------

            var studentCode =
                await GenerateStudentCodeAsync(
                    cancellationToken);


            // -------------------------------------
            // CREATE STUDENT ENTITY
            // -------------------------------------

            var student =
                new Student
                {
                    Id =
                        Guid.NewGuid(),

                    StudentCode =
                        studentCode,

                    FullName =
                        request.FullName.Trim(),

                    MobileNumber =
                        request.MobileNumber.Trim(),

                    Email =
                        Normalize(
                            request.Email),

                    GuardianName =
                        Normalize(
                            request.GuardianName),

                    GuardianMobile =
                        Normalize(
                            request.GuardianMobile),

                    EmergencyMobileNumber =
                        Normalize(
                            request.EmergencyMobileNumber),

                    DateOfBirth =
                        request.DateOfBirth,

                    Gender =
                        Normalize(
                            request.Gender),

                    NidNumber =
                        Normalize(
                            request.NidNumber),

                    Address =
                        Normalize(
                            request.Address),

                    IsBelowSsc =
                        request.IsBelowSsc,

                    AdmissionDate =
                        request.AdmissionDate,

                    PhotoPath =
                        Normalize(
                            request.PhotoPath),

                    IsActive =
                        request.IsActive,

                    IsDeleted =
                        false,

                    CreatedAt =
                        DateTime.UtcNow
                };


            // =====================================
            // EDUCATION QUALIFICATIONS
            // =====================================

            if (!request.IsBelowSsc)
            {
                foreach (var item in
                    request.EducationQualifications)
                {
                    student.EducationQualifications.Add(
                        new StudentEducationQualification
                        {
                            Id =
                                Guid.NewGuid(),

                            StudentId =
                                student.Id,

                            EducationLevel =
                                item.EducationLevel,

                            ExaminationName =
                                Normalize(
                                    item.ExaminationName),

                            PassingYear =
                                item.PassingYear,

                            Result =
                                Normalize(
                                    item.Result),

                            BoardOrUniversity =
                                Normalize(
                                    item.BoardOrUniversity),

                            InstitutionName =
                                Normalize(
                                    item.InstitutionName),

                            Remarks =
                                Normalize(
                                    item.Remarks),

                            DisplayOrder =
                                item.DisplayOrder,

                            CreatedAt =
                                DateTime.UtcNow
                        });
                }
            }


            // =====================================
            // PROFESSIONAL EXPERIENCES
            // =====================================

            foreach (var item in
                request.ProfessionalExperiences)
            {
                student.ProfessionalExperiences.Add(
                    new StudentProfessionalExperience
                    {
                        Id =
                            Guid.NewGuid(),

                        StudentId =
                            student.Id,

                        ExperienceTitle =
                            item.ExperienceTitle.Trim(),

                        OrganizationName =
                            Normalize(
                                item.OrganizationName),

                        YearsOfExperience =
                            item.YearsOfExperience,

                        Remarks =
                            Normalize(
                                item.Remarks),

                        CreatedAt =
                            DateTime.UtcNow
                    });
            }


            // =====================================
            // LANGUAGE PROFICIENCIES
            // =====================================

            foreach (var item in
                request.LanguageProficiencies)
            {
                student.LanguageProficiencies.Add(
                    new StudentLanguageProficiency
                    {
                        Id =
                            Guid.NewGuid(),

                        StudentId =
                            student.Id,

                        LanguageName =
                            item.LanguageName.Trim(),

                        ProficiencyLevel =
                            Normalize(
                                item.ProficiencyLevel),

                        InstitutionName =
                            Normalize(
                                item.InstitutionName),

                        Remarks =
                            Normalize(
                                item.Remarks),

                        CreatedAt =
                            DateTime.UtcNow
                    });
            }


            // -------------------------------------
            // SAVE STUDENT
            // -------------------------------------

            await _studentRepository.AddAsync(
                student,
                cancellationToken);

            await _studentRepository.SaveChangesAsync(
                cancellationToken);


            // -------------------------------------
            // RETURN DTO
            // -------------------------------------

            return MapToDto(student);
        }


        // =========================================
        // GET STUDENT SUMMARY
        // =========================================

        public async Task<StudentSummaryDto> GetSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            return await _studentRepository
                .GetSummaryAsync(
                    cancellationToken);
        }


        // =========================================
        // TODAY'S DUE STUDENTS
        // =========================================

        public async Task<IReadOnlyList<TodayDueStudentDto>>
            GetTodayDueStudentsAsync(
                CancellationToken cancellationToken = default)
        {
            var today =
                GetBangladeshToday();

            return await _studentRepository
                .GetDueStudentsByDateAsync(
                    today,
                    cancellationToken);
        }


        // =========================================
        // GET ALL DUE STUDENTS
        // =========================================

        public async Task<IReadOnlyList<DueStudentDto>>
            GetDueStudentsAsync(
                CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // GET ACTIVE STUDENTS
            // -------------------------------------

            var students =
                await _studentRepository
                    .GetAllAsync(
                        cancellationToken);


            var activeStudents =
                students
                    .Where(student =>
                        student.IsActive)
                    .ToList();


            var dueStudents =
                new List<DueStudentDto>();


            // -------------------------------------
            // CALCULATE EACH STUDENT'S
            // FINANCIAL POSITION
            // -------------------------------------

            foreach (var student in activeStudents)
            {
                var financialSummary =
                    await GetFinancialSummaryAsync(
                        student.Id,
                        cancellationToken);


                if (financialSummary is null)
                {
                    continue;
                }


                // Only students having an actual
                // outstanding balance belong in
                // the Due Students list.

                if (financialSummary.OutstandingAmount <= 0)
                {
                    continue;
                }


                dueStudents.Add(
                    new DueStudentDto
                    {
                        Id =
                            student.Id,

                        StudentCode =
                            student.StudentCode,

                        FullName =
                            student.FullName,

                        MobileNumber =
                            student.MobileNumber,

                        TotalPayable =
                            financialSummary.TotalPayable,

                        TotalPaid =
                            financialSummary.TotalPaid,

                        DueAmount =
                            financialSummary.OutstandingAmount,

                        NextPaymentDate =
                            financialSummary.NextPaymentDate,

                        NextPaymentAmount =
                            financialSummary.NextPaymentAmount,

                        IsDueToday =
                            financialSummary.IsDueToday,

                        IsOverdue =
                            financialSummary.IsOverdue
                    });
            }


            // -------------------------------------
            // SORTING
            //
            // 1. Overdue first
            // 2. Due today
            // 3. Earliest payment date
            // 4. Student name
            // -------------------------------------

            return dueStudents
                .OrderByDescending(student =>
                    student.IsOverdue)
                .ThenByDescending(student =>
                    student.IsDueToday)
                .ThenBy(student =>
                    student.NextPaymentDate
                    ?? DateOnly.MaxValue)
                .ThenBy(student =>
                    student.FullName)
                .ToList();
        }


        // =========================================
        // GET ALL PAID STUDENTS
        // =========================================

        public async Task<IReadOnlyList<PaidStudentDto>>
            GetPaidStudentsAsync(
                CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // GET ACTIVE STUDENTS
            // -------------------------------------

            var students =
                await _studentRepository
                    .GetAllAsync(
                        cancellationToken);


            var activeStudents =
                students
                    .Where(student =>
                        student.IsActive)
                    .ToList();


            var paidStudents =
                new List<PaidStudentDto>();


            // -------------------------------------
            // CALCULATE EACH STUDENT'S
            // FINANCIAL POSITION
            // -------------------------------------

            foreach (var student in activeStudents)
            {
                var financialSummary =
                    await GetFinancialSummaryAsync(
                        student.Id,
                        cancellationToken);


                if (financialSummary is null)
                {
                    continue;
                }


                // ---------------------------------
                // FULLY PAID RULE
                //
                // 1. Student must have liability.
                // 2. Outstanding must be zero.
                // 3. Advance must be zero.
                // ---------------------------------

                if (financialSummary.TotalPayable <= 0)
                {
                    continue;
                }

                if (financialSummary.OutstandingAmount > 0)
                {
                    continue;
                }

                if (financialSummary.AdvanceAmount > 0)
                {
                    continue;
                }


                // ---------------------------------
                // GET LAST VALID PAYMENT DATE
                // ---------------------------------

                var payments =
                    await _studentPaymentRepository
                        .GetByStudentIdAsync(
                            student.Id,
                            cancellationToken);


                var lastPaymentDate =
                    payments
                        .Where(payment =>
                            !payment.IsReversed)
                        .OrderByDescending(payment =>
                            payment.PaymentDate)
                        .Select(payment =>
                            (DateTime?)payment.PaymentDate)
                        .FirstOrDefault();


                // ---------------------------------
                // ADD TO PAID STUDENTS
                // ---------------------------------

                paidStudents.Add(
                    new PaidStudentDto
                    {
                        Id =
                            student.Id,

                        StudentCode =
                            student.StudentCode,

                        FullName =
                            student.FullName,

                        MobileNumber =
                            student.MobileNumber,

                        TotalPayable =
                            financialSummary.TotalPayable,

                        TotalPaid =
                            financialSummary.TotalPaid,

                        PaidAmount =
                            financialSummary.TotalPaid,

                        PaymentStatus =
                            "Paid",

                        LastPaymentDate =
                            lastPaymentDate
                    });
            }


            // -------------------------------------
            // SORTING
            // -------------------------------------

            return paidStudents
                .OrderByDescending(student =>
                    student.LastPaymentDate
                    ?? DateTime.MinValue)
                .ThenBy(student =>
                    student.FullName)
                .ToList();
        }


        // =========================================
        // GET STUDENT FINANCIAL SUMMARY
        // =========================================

        public async Task<StudentFinancialSummaryDto?>
            GetFinancialSummaryAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            // -------------------------------------
            // VALIDATE STUDENT ID
            // -------------------------------------

            if (studentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }


            // -------------------------------------
            // GET STUDENT
            // -------------------------------------

            var student =
                await _studentRepository
                    .GetByIdAsync(
                        studentId,
                        cancellationToken);

            if (student is null)
            {
                return null;
            }


            // -------------------------------------
            // GET FEE ASSIGNMENTS
            // -------------------------------------

            var feeAssignments =
                await _studentFeeAssignmentRepository
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);


            var activeFeeAssignments =
                feeAssignments
                    .Where(assignment =>
                        assignment.IsActive &&
                        !assignment.IsDeleted)
                    .ToList();


            // -------------------------------------
            // TOTAL LIABILITY
            //
            // Fee assignments are the source of
            // liability. Schedules never create
            // additional liability.
            // -------------------------------------

            var totalPayable =
                activeFeeAssignments
                    .Sum(assignment =>
                        assignment.NetAmount);


            // -------------------------------------
            // GET VALID TOTAL PAID
            // -------------------------------------

            var totalPaid =
                await _studentPaymentRepository
                    .GetTotalValidPaidAsync(
                        studentId,
                        cancellationToken);


            // -------------------------------------
            // CALCULATE BALANCE
            // -------------------------------------

            var balance =
                totalPayable -
                totalPaid;


            decimal outstandingAmount;
            decimal advanceAmount;
            string paymentStatus;


            // -------------------------------------
            // PAYMENT STATUS
            //
            // Professional status rules:
            //
            // 1. No fee + no payment
            //    => No Fee Assigned
            //
            // 2. No fee + payment received
            //    => Advance
            //
            // 3. Fee assigned + no payment
            //    => Unpaid
            //
            // 4. Fee assigned + partial payment
            //    => Partially Paid
            //
            // 5. Fee fully settled
            //    => Paid
            //
            // 6. Payment exceeds liability
            //    => Advance
            // -------------------------------------

            if (
                totalPayable <= 0 &&
                totalPaid <= 0)
            {
                outstandingAmount =
                    0m;

                advanceAmount =
                    0m;

                paymentStatus =
                    "No Fee Assigned";
            }
            else if (
                totalPayable <= 0 &&
                totalPaid > 0)
            {
                outstandingAmount =
                    0m;

                advanceAmount =
                    totalPaid;

                paymentStatus =
                    "Advance";
            }
            else if (balance < 0)
            {
                outstandingAmount =
                    0m;

                advanceAmount =
                    Math.Abs(balance);

                paymentStatus =
                    "Advance";
            }
            else if (balance == 0)
            {
                outstandingAmount =
                    0m;

                advanceAmount =
                    0m;

                paymentStatus =
                    "Paid";
            }
            else if (totalPaid <= 0)
            {
                outstandingAmount =
                    balance;

                advanceAmount =
                    0m;

                paymentStatus =
                    "Unpaid";
            }
            else
            {
                outstandingAmount =
                    balance;

                advanceAmount =
                    0m;

                paymentStatus =
                    "Partially Paid";
            }


            // -------------------------------------
            // GET PAYMENT SCHEDULES
            // -------------------------------------

            var schedules =
                await _paymentScheduleRepository
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);


            var today =
                GetBangladeshToday();


            // -------------------------------------
            // ACTIVE SCHEDULES
            // -------------------------------------

            var activeSchedules =
                schedules
                    .Where(schedule =>
                        schedule.IsActive &&
                        !schedule.IsDeleted)
                    .ToList();


            // -------------------------------------
            // CALCULATE OPEN SCHEDULES
            //
            // Reversed payment allocations do not
            // count as paid.
            // -------------------------------------

            var openSchedules =
                activeSchedules
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
                    .OrderBy(item =>
                        item.Schedule.DueDate)
                    .ThenBy(item =>
                        item.Schedule.CreatedAt)
                    .ToList();


            // =====================================
            // UNSCHEDULED FEE ASSIGNMENTS
            //
            // Assignment DueDate is authoritative
            // only when the assignment has no
            // active/non-deleted schedule.
            // =====================================

            var unscheduledAssignments =
                activeFeeAssignments
                    .Where(assignment =>
                        assignment.DueDate.HasValue &&
                        !activeSchedules.Any(schedule =>
                            schedule.StudentFeeAssignmentId ==
                                assignment.Id))
                    .OrderBy(assignment =>
                        assignment.DueDate)
                    .ThenBy(assignment =>
                        assignment.CreatedAt)
                    .ToList();


            // =====================================
            // BUILD PAYMENT-DATE CANDIDATES
            //
            // These represent payment timing only.
            // They do NOT create liability.
            // =====================================

            var paymentDateCandidates =
                new List<PaymentDateCandidate>();


            // -------------------------------------
            // SCHEDULE CANDIDATES
            // -------------------------------------

            foreach (var item in openSchedules)
            {
                paymentDateCandidates.Add(
                    new PaymentDateCandidate
                    {
                        DueDate =
                            item.Schedule.DueDate,

                        Amount =
                            item.RemainingAmount,

                        CreatedAt =
                            item.Schedule.CreatedAt
                    });
            }


            // -------------------------------------
            // ASSIGNMENT FALLBACK CANDIDATES
            //
            // Only include these while the student
            // actually has an outstanding balance.
            // -------------------------------------

            if (outstandingAmount > 0)
            {
                foreach (var assignment in
                    unscheduledAssignments)
                {
                    paymentDateCandidates.Add(
                        new PaymentDateCandidate
                        {
                            DueDate =
                                assignment.DueDate!.Value,

                            Amount =
                                assignment.NetAmount,

                            CreatedAt =
                                assignment.CreatedAt
                        });
                }
            }


            // -------------------------------------
            // ORDER ALL PAYMENT DATES
            // -------------------------------------

            paymentDateCandidates =
                paymentDateCandidates
                    .OrderBy(item =>
                        item.DueDate)
                    .ThenBy(item =>
                        item.CreatedAt)
                    .ToList();


            // =====================================
            // NEXT PAYMENT
            //
            // Earliest unpaid / partial payment
            // timing is authoritative.
            //
            // Overdue items remain the next
            // payment until they are settled.
            // =====================================

            var nextPaymentCandidate =
                paymentDateCandidates
                    .FirstOrDefault();


            var nextPaymentDate =
                nextPaymentCandidate?.DueDate;


            decimal nextPaymentAmount =
                0m;


            if (nextPaymentDate.HasValue)
            {
                nextPaymentAmount =
                    paymentDateCandidates
                        .Where(item =>
                            item.DueDate ==
                            nextPaymentDate.Value)
                        .Sum(item =>
                            item.Amount);


                // Timing candidates must never
                // report more than the student's
                // actual outstanding liability.

                nextPaymentAmount =
                    Math.Min(
                        nextPaymentAmount,
                        outstandingAmount);
            }


            // =====================================
            // DUE TODAY
            // =====================================

            var isDueToday =
                outstandingAmount > 0 &&
                paymentDateCandidates.Any(item =>
                    item.DueDate ==
                    today);


            // =====================================
            // OVERDUE
            // =====================================

            var isOverdue =
                outstandingAmount > 0 &&
                paymentDateCandidates.Any(item =>
                    item.DueDate <
                    today);


            // -------------------------------------
            // RETURN FINANCIAL SUMMARY
            // -------------------------------------

            return new StudentFinancialSummaryDto
            {
                StudentId =
                    student.Id,

                StudentCode =
                    student.StudentCode,

                StudentName =
                    student.FullName,

                TotalPayable =
                    totalPayable,

                TotalPaid =
                    totalPaid,

                OutstandingAmount =
                    outstandingAmount,

                AdvanceAmount =
                    advanceAmount,

                PaymentStatus =
                    paymentStatus,

                NextPaymentDate =
                    nextPaymentDate,

                NextPaymentAmount =
                    nextPaymentAmount,

                IsDueToday =
                    isDueToday,

                IsOverdue =
                    isOverdue
            };
        }


        // =========================================
        // PAYMENT DATE CANDIDATE
        //
        // Internal calculation model only.
        // Does not represent a database entity.
        // =========================================

        private sealed class PaymentDateCandidate
        {
            public DateOnly DueDate { get; set; }

            public decimal Amount { get; set; }

            public DateTime CreatedAt { get; set; }
        }


        // =========================================
        // GENERATE STUDENT CODE
        // =========================================

        private async Task<string>
            GenerateStudentCodeAsync(
                CancellationToken cancellationToken)
        {
            string code;

            do
            {
                code =
                    $"AKLC-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            }
            while (
                await _studentRepository
                    .ExistsByStudentCodeAsync(
                        code,
                        cancellationToken)
            );

            return code;
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static StudentDto MapToDto(
            Student student)
        {
            return new StudentDto
            {
                Id =
                    student.Id,

                StudentCode =
                    student.StudentCode,

                FullName =
                    student.FullName,

                MobileNumber =
                    student.MobileNumber,

                Email =
                    student.Email,

                GuardianName =
                    student.GuardianName,

                GuardianMobile =
                    student.GuardianMobile,

                EmergencyMobileNumber =
                    student.EmergencyMobileNumber,

                DateOfBirth =
                    student.DateOfBirth,

                Gender =
                    student.Gender,

                NidNumber =
                    student.NidNumber,

                Address =
                    student.Address,

                IsBelowSsc =
                    student.IsBelowSsc,

                AdmissionDate =
                    student.AdmissionDate,

                IsActive =
                    student.IsActive,

                PhotoPath =
                    student.PhotoPath,

                EducationQualifications =
                    student.EducationQualifications
                        .OrderBy(x =>
                            x.DisplayOrder)
                        .ThenBy(x =>
                            x.EducationLevel)
                        .Select(x =>
                            new StudentEducationDto
                            {
                                Id =
                                    x.Id,

                                EducationLevel =
                                    x.EducationLevel,

                                ExaminationName =
                                    x.ExaminationName,

                                PassingYear =
                                    x.PassingYear,

                                Result =
                                    x.Result,

                                BoardOrUniversity =
                                    x.BoardOrUniversity,

                                InstitutionName =
                                    x.InstitutionName,

                                Remarks =
                                    x.Remarks,

                                DisplayOrder =
                                    x.DisplayOrder
                            })
                        .ToList(),

                ProfessionalExperiences =
                    student.ProfessionalExperiences
                        .Select(x =>
                            new StudentProfessionalExperienceDto
                            {
                                Id =
                                    x.Id,

                                ExperienceTitle =
                                    x.ExperienceTitle,

                                OrganizationName =
                                    x.OrganizationName,

                                YearsOfExperience =
                                    x.YearsOfExperience,

                                Remarks =
                                    x.Remarks
                            })
                        .ToList(),

                LanguageProficiencies =
                    student.LanguageProficiencies
                        .Select(x =>
                            new StudentLanguageProficiencyDto
                            {
                                Id =
                                    x.Id,

                                LanguageName =
                                    x.LanguageName,

                                ProficiencyLevel =
                                    x.ProficiencyLevel,

                                InstitutionName =
                                    x.InstitutionName,

                                Remarks =
                                    x.Remarks
                            })
                        .ToList()
            };
        }


        // =========================================
        // BANGLADESH BUSINESS DATE
        // =========================================

        private static DateOnly
            GetBangladeshToday()
        {
            TimeZoneInfo timeZone;

            try
            {
                // Windows
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Bangladesh Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                // Linux fallback
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Dhaka");
            }
            catch (InvalidTimeZoneException)
            {
                // Linux fallback
                timeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Asia/Dhaka");
            }


            var bangladeshNow =
                TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    timeZone);


            return DateOnly.FromDateTime(
                bangladeshNow);
        }


        // =========================================
        // NORMALIZE OPTIONAL STRING
        // =========================================

        private static string? Normalize(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}