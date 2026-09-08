using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IPaymentAttachmentRepository
    {
        Task AddAsync(
            PaymentAttachment attachment,
            CancellationToken cancellationToken = default);

        Task AddRangeAsync(
            IEnumerable<PaymentAttachment> attachments,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentAttachment>> GetByPaymentIdAsync(
            Guid studentPaymentId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}