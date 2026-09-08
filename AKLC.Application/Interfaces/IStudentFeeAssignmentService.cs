using AKLC.Application.DTOs.StudentFeeAssignments;

namespace AKLC.Application.Interfaces
{
    public interface IStudentFeeAssignmentService
    {
        Task<StudentFeeAssignmentDto> CreateAsync(
            CreateStudentFeeAssignmentRequest request,
            CancellationToken cancellationToken = default);

        Task<StudentFeeAssignmentDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<StudentFeeAssignmentDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);
    }
}