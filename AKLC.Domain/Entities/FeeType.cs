using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class FeeType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<StudentFeeAssignment> StudentFeeAssignments { get; set; }
            = new List<StudentFeeAssignment>();

        public ICollection<CourseFeeStructure> CourseFeeStructures { get; set; }
            = new List<CourseFeeStructure>();
    }
}