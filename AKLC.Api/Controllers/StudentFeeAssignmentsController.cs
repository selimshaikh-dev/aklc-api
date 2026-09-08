using AKLC.Application.DTOs.StudentFeeAssignments;
using AKLC.Application.Interfaces;
using AKLC.Domain.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AKLC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = AppRoles.Admin)]
    public class StudentFeeAssignmentsController
        : ControllerBase
    {
        private readonly IStudentFeeAssignmentService
            _studentFeeAssignmentService;


        // =========================================
        // CONSTRUCTOR
        // =========================================

        public StudentFeeAssignmentsController(
            IStudentFeeAssignmentService studentFeeAssignmentService)
        {
            _studentFeeAssignmentService =
                studentFeeAssignmentService;
        }


        // =========================================
        // GET FEE ASSIGNMENT BY ID
        // GET: api/StudentFeeAssignments/{id}
        // =========================================

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentFeeAssignmentDto>>
            GetById(
                Guid id,
                CancellationToken cancellationToken)
        {
            var assignment =
                await _studentFeeAssignmentService
                    .GetByIdAsync(
                        id,
                        cancellationToken);

            if (assignment is null)
            {
                return NotFound(
                    new
                    {
                        message =
                            "Student fee assignment not found."
                    });
            }

            return Ok(
                assignment);
        }


        // =========================================
        // GET FEE ASSIGNMENTS BY STUDENT
        // GET: api/StudentFeeAssignments/student/{studentId}
        // =========================================

        [HttpGet("student/{studentId:guid}")]
        public async Task<
            ActionResult<IReadOnlyList<StudentFeeAssignmentDto>>>
            GetByStudentId(
                Guid studentId,
                CancellationToken cancellationToken)
        {
            var assignments =
                await _studentFeeAssignmentService
                    .GetByStudentIdAsync(
                        studentId,
                        cancellationToken);

            return Ok(
                assignments);
        }


        // =========================================
        // CREATE FEE ASSIGNMENT
        // POST: api/StudentFeeAssignments
        // =========================================

        [HttpPost]
        public async Task<ActionResult<StudentFeeAssignmentDto>>
            Create(
                [FromBody]
                CreateStudentFeeAssignmentRequest request,
                CancellationToken cancellationToken)
        {
            var assignment =
                await _studentFeeAssignmentService
                    .CreateAsync(
                        request,
                        cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id = assignment.Id
                },
                assignment);
        }
    }
}