using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IStudentPaymentRepository
    {
        // =========================================
        // ADD PAYMENT
        // =========================================

        Task AddAsync(
            StudentPayment payment,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET PAYMENT BY ID - READ ONLY
        // =========================================

        Task<StudentPayment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET PAYMENT BY ID - FOR UPDATE
        // =========================================

        Task<StudentPayment?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET PAYMENT HISTORY BY STUDENT
        // =========================================

        Task<IReadOnlyList<StudentPayment>> GetByStudentIdAsync(
            Guid studentId,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET VALID PAYMENTS WITH
        // UNALLOCATED BALANCE
        // =========================================

        Task<IReadOnlyList<StudentPayment>>
            GetValidPaymentsWithUnallocatedBalanceAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);


        // =========================================
        // GET TOTAL VALID PAID AMOUNT
        // =========================================

        Task<decimal> GetTotalValidPaidAsync(
            Guid studentId,
            CancellationToken cancellationToken = default);


        // =========================================
        // SAVE CHANGES
        // =========================================

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}