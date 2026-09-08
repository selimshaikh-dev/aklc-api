using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class PaymentAttachmentRepository
        : IPaymentAttachmentRepository
    {
        private readonly ApplicationDbContext
            _context;


        public PaymentAttachmentRepository(
            ApplicationDbContext context)
        {
            _context =
                context;
        }


        // =========================================
        // ADD ATTACHMENT
        // =========================================

        public async Task AddAsync(
            PaymentAttachment attachment,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentAttachments
                .AddAsync(
                    attachment,
                    cancellationToken);
        }


        // =========================================
        // ADD MULTIPLE ATTACHMENTS
        // =========================================

        public async Task AddRangeAsync(
            IEnumerable<PaymentAttachment> attachments,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentAttachments
                .AddRangeAsync(
                    attachments,
                    cancellationToken);
        }


        // =========================================
        // GET ATTACHMENTS BY PAYMENT
        // =========================================

        public async Task<IReadOnlyList<PaymentAttachment>>
            GetByPaymentIdAsync(
                Guid studentPaymentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.PaymentAttachments
                .AsNoTracking()
                .Where(x =>
                    x.StudentPaymentId ==
                    studentPaymentId)
                .OrderBy(x =>
                    x.CreatedAt)
                .ToListAsync(
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
    }
}