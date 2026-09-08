using AKLC.Application.DTOs.StudentPayments;

namespace AKLC.Application.Interfaces
{
    public interface IStudentPaymentService
    {
        // =========================================
        // CREATE PAYMENT
        // =========================================

        Task<StudentPaymentDto> CreateAsync(
            CreateStudentPaymentRequest request,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET PAYMENT BY ID
        // =========================================

        Task<StudentPaymentDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET PAYMENTS BY STUDENT
        // =========================================

        Task<IReadOnlyList<StudentPaymentDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);


        // =========================================
        // REVERSE PAYMENT
        // =========================================

        Task<StudentPaymentDto> ReverseAsync(
            Guid paymentId,
            ReverseStudentPaymentRequest request,
            Guid reversedByUserId,
            CancellationToken cancellationToken = default);
    }
}