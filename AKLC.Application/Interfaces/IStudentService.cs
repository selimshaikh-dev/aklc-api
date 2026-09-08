using AKLC.Application.DTOs.Students;

namespace AKLC.Application.Interfaces
{
    public interface IStudentService
    {
        // =========================================
        // GET ALL STUDENTS
        // =========================================

        Task<IReadOnlyList<StudentDto>> GetAllAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // GET STUDENT BY ID
        // =========================================

        Task<StudentDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);


        // =========================================
        // CREATE STUDENT
        // =========================================

        Task<StudentDto> CreateAsync(
            CreateStudentRequest request,
            CancellationToken cancellationToken = default);


        // =========================================
        // STUDENT SUMMARY
        // =========================================

        Task<StudentSummaryDto> GetSummaryAsync(
            CancellationToken cancellationToken = default);


        // =========================================
        // TODAY DUE STUDENTS
        // =========================================

        Task<IReadOnlyList<TodayDueStudentDto>>
            GetTodayDueStudentsAsync(
                CancellationToken cancellationToken = default);


        // =========================================
        // DUE STUDENTS
        // =========================================

        Task<IReadOnlyList<DueStudentDto>>
            GetDueStudentsAsync(
                CancellationToken cancellationToken = default);


        // =========================================
        // PAID STUDENTS
        // =========================================

        Task<IReadOnlyList<PaidStudentDto>>
            GetPaidStudentsAsync(
                CancellationToken cancellationToken = default);


        // =========================================
        // STUDENT FINANCIAL SUMMARY
        // =========================================

        Task<StudentFinancialSummaryDto?>
            GetFinancialSummaryAsync(
                Guid studentId,
                CancellationToken cancellationToken = default);
    }
}