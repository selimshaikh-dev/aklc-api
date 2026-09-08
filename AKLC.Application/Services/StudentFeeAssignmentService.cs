using AKLC.Application.DTOs.StudentFeeAssignments;
using AKLC.Application.Interfaces;
using AKLC.Domain.Entities;

namespace AKLC.Application.Services
{
    public class StudentFeeAssignmentService
        : IStudentFeeAssignmentService
    {
        private readonly IStudentFeeAssignmentRepository
            _studentFeeAssignmentRepository;

        private readonly IStudentRepository
            _studentRepository;

        private readonly IFeeTypeRepository
            _feeTypeRepository;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentFeeAssignmentService(
            IStudentFeeAssignmentRepository studentFeeAssignmentRepository,
            IStudentRepository studentRepository,
            IFeeTypeRepository feeTypeRepository)
        {
            _studentFeeAssignmentRepository =
                studentFeeAssignmentRepository;

            _studentRepository =
                studentRepository;

            _feeTypeRepository =
                feeTypeRepository;
        }


        // =========================================
        // CREATE FEE ASSIGNMENT
        // =========================================

        public async Task<StudentFeeAssignmentDto>
            CreateAsync(
                CreateStudentFeeAssignmentRequest request,
                CancellationToken cancellationToken = default)
        {
            // =====================================
            // BASIC VALIDATION
            // =====================================

            if (request.StudentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }

            if (request.FeeTypeId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Fee type is required.");
            }

            if (request.Amount <= 0)
            {
                throw new ArgumentException(
                    "Amount must be greater than zero.");
            }

            if (request.DiscountAmount < 0)
            {
                throw new ArgumentException(
                    "Discount amount cannot be negative.");
            }

            if (request.DiscountAmount > request.Amount)
            {
                throw new ArgumentException(
                    "Discount amount cannot exceed the fee amount.");
            }


            // =====================================
            // VALIDATE STUDENT
            // =====================================

            var student =
                await _studentRepository
                    .GetByIdAsync(
                        request.StudentId,
                        cancellationToken);

            if (student is null)
            {
                throw new KeyNotFoundException(
                    "Student was not found.");
            }

            if (!student.IsActive)
            {
                throw new InvalidOperationException(
                    "Fee cannot be assigned to an inactive student.");
            }


            // =====================================
            // VALIDATE FEE TYPE
            // =====================================

            var feeType =
                await _feeTypeRepository
                    .GetByIdAsync(
                        request.FeeTypeId,
                        cancellationToken);

            if (feeType is null)
            {
                throw new KeyNotFoundException(
                    "Fee type was not found.");
            }

            if (!feeType.IsActive ||
                feeType.IsDeleted)
            {
                throw new InvalidOperationException(
                    "The fee type is not active.");
            }


            // =====================================
            // PREVENT DUPLICATE ASSIGNMENT
            // =====================================

            var alreadyExists =
                await _studentFeeAssignmentRepository
                    .ExistsAsync(
                        request.StudentId,
                        request.FeeTypeId,
                        cancellationToken);

            if (alreadyExists)
            {
                throw new InvalidOperationException(
                    "This fee type is already assigned to the student.");
            }


            // =====================================
            // CALCULATE NET AMOUNT ON SERVER
            // =====================================

            var netAmount =
                request.Amount -
                request.DiscountAmount;


            // =====================================
            // CREATE ENTITY
            // =====================================

            var assignment =
                new StudentFeeAssignment
                {
                    StudentId =
                        request.StudentId,

                    FeeTypeId =
                        request.FeeTypeId,

                    Amount =
                        request.Amount,

                    DiscountAmount =
                        request.DiscountAmount,

                    NetAmount =
                        netAmount,

                    DueDate =
                        request.DueDate,

                    Remarks =
                        NormalizeOptionalText(
                            request.Remarks),

                    IsActive =
                        true,

                    IsDeleted =
                        false
                };


            // =====================================
            // SAVE
            // =====================================

            await _studentFeeAssignmentRepository
                .AddAsync(
                    assignment,
                    cancellationToken);

            await _studentFeeAssignmentRepository
                .SaveChangesAsync(
                    cancellationToken);


            // =====================================
            // RETURN DTO
            // =====================================

            return MapToDto(
                assignment,
                student,
                feeType);
        }


        // =========================================
        // GET BY ID
        // =========================================

        public async Task<StudentFeeAssignmentDto?>
            GetByIdAsync(
                Guid id,
                CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                return null;
            }

            var assignment =
                await _studentFeeAssignmentRepository
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (assignment is null)
            {
                return null;
            }

            return MapToDto(
                assignment,
                assignment.Student,
                assignment.FeeType);
        }


        // =========================================
        // GET BY STUDENT ID
        // =========================================

        public async Task<IReadOnlyList<StudentFeeAssignmentDto>>
            GetByStudentIdAsync(
                Guid studentId,
                CancellationToken cancellationToken = default)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Student is required.");
            }

            var assignments =
                await _studentFeeAssignmentRepository
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);

            return assignments
                .Select(
                    assignment =>
                        MapToDto(
                            assignment,
                            assignment.Student,
                            assignment.FeeType))
                .ToList();
        }


        // =========================================
        // MAP ENTITY TO DTO
        // =========================================

        private static StudentFeeAssignmentDto
            MapToDto(
                StudentFeeAssignment assignment,
                Student student,
                FeeType feeType)
        {
            return new StudentFeeAssignmentDto
            {
                Id =
                    assignment.Id,

                StudentId =
                    assignment.StudentId,

                StudentCode =
                    student.StudentCode,

                StudentName =
                    student.FullName,

                FeeTypeId =
                    assignment.FeeTypeId,

                FeeTypeName =
                    feeType.Name,

                Amount =
                    assignment.Amount,

                DiscountAmount =
                    assignment.DiscountAmount,

                NetAmount =
                    assignment.NetAmount,

                DueDate =
                    assignment.DueDate,

                Remarks =
                    assignment.Remarks,

                CreatedAt =
                    assignment.CreatedAt,

                UpdatedAt =
                    assignment.UpdatedAt
            };
        }


        // =========================================
        // NORMALIZE OPTIONAL TEXT
        // =========================================

        private static string?
            NormalizeOptionalText(
                string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }
    }
}