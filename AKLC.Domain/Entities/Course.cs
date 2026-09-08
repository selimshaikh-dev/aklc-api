using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public string? Code { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<Batch> Batches { get; set; }
            = new List<Batch>();

        public ICollection<Student> Students { get; set; }
            = new List<Student>();

        public ICollection<CourseFeeStructure> FeeStructures { get; set; }
            = new List<CourseFeeStructure>();
    }
}