using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class PaymentScheduleRepository
        : IPaymentScheduleRepository
    {
        private readonly ApplicationDbContext
            _context;

        public PaymentScheduleRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            PaymentSchedule schedule,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentSchedules
                .AddAsync(
                    schedule,
                    cancellationToken);
        }


        // =========================================
        // GET BY ID - READ ONLY
        // =========================================

        public async Task<PaymentSchedule?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentSchedules
                .AsNoTracking()

                .Include(x => x.Student)

                .Include(x => x.StudentFeeAssignment)
                    .ThenInclude(x => x!.FeeType)

                .Include(x => x.Allocations)
                    .ThenInclude(x => x.StudentPayment)

                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // GET BY ID - TRACKED FOR UPDATE
        // =========================================

        public async Task<PaymentSchedule?>
            GetByIdForUpdateAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentSchedules

                .Include(x => x.Student)

                .Include(x => x.StudentFeeAssignment)
                    .ThenInclude(x => x!.FeeType)

                .Include(x => x.Allocations)
                    .ThenInclude(x => x.StudentPayment)

                .FirstOrDefaultAsync(
                    x =>
                        x.Id == id &&
                        !x.IsDeleted,
                    cancellationToken);
        }


        // =========================================
        // GET SCHEDULES BY STUDENT
        // =========================================

        public async Task<IReadOnlyList<PaymentSchedule>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentSchedules
                .AsNoTracking()

                .Include(x => x.Student)

                .Include(x => x.StudentFeeAssignment)
                    .ThenInclude(x => x!.FeeType)

                .Include(x => x.Allocations)
                    .ThenInclude(x => x.StudentPayment)

                .Where(
                    x =>
                        x.StudentId == studentId &&
                        !x.IsDeleted)

                .OrderBy(x => x.DueDate)

                .ThenBy(x => x.CreatedAt)

                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET SCHEDULES BY FEE ASSIGNMENT
        // =========================================

        public async Task<IReadOnlyList<PaymentSchedule>>
            GetByFeeAssignmentIdAsync(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentSchedules
                .AsNoTracking()

                .Include(x => x.Student)

                .Include(x => x.StudentFeeAssignment)
                    .ThenInclude(x => x!.FeeType)

                .Include(x => x.Allocations)
                    .ThenInclude(x => x.StudentPayment)

                .Where(
                    x =>
                        x.StudentFeeAssignmentId ==
                            studentFeeAssignmentId &&
                        !x.IsDeleted)

                .OrderBy(x => x.DueDate)

                .ThenBy(x => x.CreatedAt)

                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // TOTAL SCHEDULED AMOUNT
        // FOR ONE FEE ASSIGNMENT
        //
        // IMPORTANT:
        // ALL NON-DELETED SCHEDULES ARE INCLUDED.
        //
        // INACTIVE SCHEDULES MAY STILL HAVE
        // FINANCIAL HISTORY / ALLOCATIONS.
        // =========================================

        public async Task<decimal>
            GetTotalScheduledAmountAsync(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentSchedules
                .AsNoTracking()

                .Where(
                    x =>
                        x.StudentFeeAssignmentId ==
                            studentFeeAssignmentId &&
                        !x.IsDeleted)

                .SumAsync(
                    x => (decimal?)x.ScheduledAmount,
                    cancellationToken)
                ?? 0m;
        }


        // =========================================
        // SAVE
        // =========================================

        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}