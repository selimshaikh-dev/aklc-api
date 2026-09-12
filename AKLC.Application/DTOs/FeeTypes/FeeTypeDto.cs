namespace AKLC.Application.DTOs.FeeTypes
{
    public class FeeTypeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } =
            string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}