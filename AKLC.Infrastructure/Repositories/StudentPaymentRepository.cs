using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class StudentPaymentRepository
        : IStudentPaymentRepository
    {
        private readonly ApplicationDbContext
            _context;


        public StudentPaymentRepository(
            ApplicationDbContext context)
        {
            _context =
                context;
        }


        // =========================================
        // ADD PAYMENT
        // =========================================

        public async Task AddAsync(
            StudentPayment payment,
            CancellationToken cancellationToken = default)
        {
            await _context.StudentPayments
                .AddAsync(
                    payment,
                    cancellationToken);
        }


        // =========================================
        // GET PAYMENT BY ID - READ ONLY
        // =========================================

        public async Task<StudentPayment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentPayments
                .AsNoTracking()
                .Include(x => x.Attachments)
                .Include(x => x.Allocations)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // GET PAYMENT BY ID - FOR UPDATE
        // =========================================

        public async Task<StudentPayment?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentPayments
                .Include(x => x.Attachments)
                .Include(x => x.Allocations)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // GET PAYMENTS BY STUDENT
        // =========================================

        public async Task<IReadOnlyList<StudentPayment>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentPayments
                .AsNoTracking()
                .Include(x => x.Attachments)
                .Include(x => x.Allocations)
                .Where(x =>
                    x.StudentId == studentId)
                .OrderByDescending(x =>
                    x.PaymentDate)
                .ThenByDescending(x =>
                    x.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET VALID PAYMENTS WITH
        // UNALLOCATED BALANCE
        //
        // ONLY NON-REVERSED PAYMENTS
        // WITH AVAILABLE CREDIT
        // =========================================

        public async Task<IReadOnlyList<StudentPayment>>
            GetValidPaymentsWithUnallocatedBalanceAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentPayments

                // Tracking is intentional.
                // Existing payments may receive
                // new allocation records.
                .Include(x => x.Allocations)

                .Where(x =>
                    x.StudentId == studentId &&
                    !x.IsReversed &&
                    x.Amount >
                        (
                            x.Allocations
                                .Sum(a =>
                                    (decimal?)
                                    a.AllocatedAmount)
                            ?? 0m
                        ))

                // Oldest available credit first.
                .OrderBy(x =>
                    x.PaymentDate)

                .ThenBy(x =>
                    x.CreatedAt)

                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET TOTAL VALID PAID
        // REVERSED PAYMENTS EXCLUDED
        // =========================================

        public async Task<decimal>
            GetTotalValidPaidAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentPayments
                .AsNoTracking()
                .Where(x =>
                    x.StudentId == studentId &&
                    !x.IsReversed)
                .SumAsync(
                    x => (decimal?)x.Amount,
                    cancellationToken)
                ?? 0m;
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
    }
}