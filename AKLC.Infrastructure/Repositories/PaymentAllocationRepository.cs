using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class PaymentAllocationRepository
        : IPaymentAllocationRepository
    {
        private readonly ApplicationDbContext
            _context;

        public PaymentAllocationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            PaymentAllocation allocation,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentAllocations
                .AddAsync(
                    allocation,
                    cancellationToken);
        }


        // =========================================
        // ADD RANGE
        // =========================================

        public async Task AddRangeAsync(
            IEnumerable<PaymentAllocation> allocations,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentAllocations
                .AddRangeAsync(
                    allocations,
                    cancellationToken);
        }


        // =========================================
        // GET ALLOCATIONS BY PAYMENT
        // =========================================

        public async Task<IReadOnlyList<PaymentAllocation>>
            GetByPaymentIdAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .AsNoTracking()
                .Include(x => x.StudentPayment)
                .Include(x => x.PaymentSchedule)
                .Where(
                    x =>
                        x.StudentPaymentId ==
                        studentPaymentId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // GET ALLOCATIONS BY SCHEDULE
        // =========================================

        public async Task<IReadOnlyList<PaymentAllocation>>
            GetByScheduleIdAsync(
                Guid paymentScheduleId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .AsNoTracking()
                .Include(x => x.StudentPayment)
                .Include(x => x.PaymentSchedule)
                .Where(
                    x =>
                        x.PaymentScheduleId ==
                        paymentScheduleId)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // VALID ALLOCATED AMOUNT OF A SCHEDULE
        // REVERSED PAYMENTS ARE EXCLUDED
        // =========================================

        public async Task<decimal>
            GetValidAllocatedAmountAsync(
                Guid paymentScheduleId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .AsNoTracking()
                .Where(
                    x =>
                        x.PaymentScheduleId ==
                            paymentScheduleId &&
                        !x.StudentPayment.IsReversed)
                .SumAsync(
                    x => (decimal?)x.AllocatedAmount,
                    cancellationToken)
                ?? 0m;
        }


        // =========================================
        // TOTAL ALLOCATED FROM ONE PAYMENT
        // =========================================

        public async Task<decimal>
            GetTotalAllocatedFromPaymentAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAllocations
                .AsNoTracking()
                .Where(
                    x =>
                        x.StudentPaymentId ==
                        studentPaymentId)
                .SumAsync(
                    x => (decimal?)x.AllocatedAmount,
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