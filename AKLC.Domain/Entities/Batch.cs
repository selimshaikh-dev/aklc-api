using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class Batch : BaseEntity
    {
        public Guid CourseId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Course Course { get; set; } = null!;
    }
}