using AKLC.Application.DTOs.PaymentSchedules;

namespace AKLC.Application.Interfaces
{
    public interface IPaymentScheduleService
    {
        Task<PaymentScheduleDto> CreateAsync(
            CreatePaymentScheduleRequest request,
            CancellationToken cancellationToken = default);

        Task<PaymentScheduleDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentScheduleDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentScheduleDto>>
            GetByFeeAssignmentIdAsync(
                Guid studentFeeAssignmentId,
                CancellationToken cancellationToken = default);
    }
}