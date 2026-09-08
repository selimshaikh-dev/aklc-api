using AKLC.Application.DTOs.Students;
using AKLC.Domain.Entities;

namespace AKLC.Application.Interfaces
{
    public interface IStudentRepository
    {
        // =========================================
        // GET ALL STUDENTS - READ ONLY
        // =========================================

        Task<IReadOnlyList<Student>> GetAllAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // GET STUDENT BY ID - READ ONLY
        // =========================================

        Task<Student?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // GET STUDENT FOR UPDATE - TRACKED
        // =========================================

        Task<Student?> GetByIdForUpdateAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // ADD STUDENT
        // =========================================

        Task AddAsync(
            Student student,
            CancellationToken cancellationToken = default);


        // =========================================
        // CHECK STUDENT CODE
        // =========================================

        Task<bool> ExistsByStudentCodeAsync(
            string studentCode,
            CancellationToken cancellationToken = default);


        // =========================================
        // SAVE CHANGES
        // =========================================

        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // STUDENT SUMMARY
        // =========================================

        Task<StudentSummaryDto> GetSummaryAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // GET DUE STUDENTS BY PAYMENT DATE
        // READ ONLY
        // =========================================

        Task<IReadOnlyList<TodayDueStudentDto>>
            GetDueStudentsByDateAsync(
                DateOnly date,
                CancellationToken cancellationToken = default);
    }
}