using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class CourseFeeStructure : BaseEntity
    {
        public Guid CourseId { get; set; }

        public Guid FeeTypeId { get; set; }

        public decimal DefaultAmount { get; set; }

        public bool IsRequired { get; set; } = true;

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Course Course { get; set; } = null!;

        public FeeType FeeType { get; set; } = null!;
    }
}