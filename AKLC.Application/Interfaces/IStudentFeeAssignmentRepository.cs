using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IStudentFeeAssignmentRepository
    {
        Task AddAsync(
            StudentFeeAssignment assignment,
            CancellationToken cancellationToken = default);

        Task<StudentFeeAssignment?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<StudentFeeAssignment?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<StudentFeeAssignment>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            Guid studentId,
            Guid feeTypeId,
            CancellationToken cancellationToken = default);

        Task<decimal> GetTotalNetPayableAsync(
            Guid studentId,
            CancellationToken cancellationToken = default);

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}