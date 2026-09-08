using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace AKLC.Infrastructure.Repositories
{
    public class StudentFeeAssignmentRepository
        : IStudentFeeAssignmentRepository
    {
        private readonly ApplicationDbContext
            _context;

        public StudentFeeAssignmentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================================
        // ADD
        // =========================================

        public async Task AddAsync(
            StudentFeeAssignment assignment,
            CancellationToken cancellationToken = default)
        {
            await _context.StudentFeeAssignments
                .AddAsync(
                    assignment,
                    cancellationToken);
        }


        // =========================================
        // GET BY ID - READ ONLY
        // =========================================

        public async Task<StudentFeeAssignment?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentFeeAssignments
                .AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.FeeType)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // GET BY ID - TRACKED FOR UPDATE
        // =========================================

        public async Task<StudentFeeAssignment?>
            GetByIdForUpdateAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentFeeAssignments
                .Include(x => x.Student)
                .Include(x => x.FeeType)
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }


        // =========================================
        // GET ALL ASSIGNMENTS OF A STUDENT
        // =========================================

        public async Task<IReadOnlyList<StudentFeeAssignment>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentFeeAssignments
                .AsNoTracking()
                .Include(x => x.Student)
                .Include(x => x.FeeType)
                .Where(
                    x => x.StudentId == studentId)
                .OrderBy(x => x.DueDate)
                .ThenBy(x => x.CreatedAt)
                .ToListAsync(
                    cancellationToken);
        }


        // =========================================
        // DUPLICATE CHECK
        // Student + FeeType
        // =========================================

        public async Task<bool> ExistsAsync(
            Guid studentId,
            Guid feeTypeId,
            CancellationToken cancellationToken = default)
        {
            return await _context.StudentFeeAssignments
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.StudentId == studentId &&
                        x.FeeTypeId == feeTypeId,
                    cancellationToken);
        }


        // =========================================
        // TOTAL NET PAYABLE
        //
        // ONLY ACTIVE + NON-DELETED ASSIGNMENTS
        // CREATE CURRENT STUDENT LIABILITY
        // =========================================

        public async Task<decimal>
            GetTotalNetPayableAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            return await _context.StudentFeeAssignments
                .AsNoTracking()
                .Where(
                    x =>
                        x.StudentId == studentId &&
                        x.IsActive &&
                        !x.IsDeleted)
                .SumAsync(
                    x => (decimal?)x.NetAmount,
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