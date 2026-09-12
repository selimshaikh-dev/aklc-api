namespace AKLC.Application.DTOs.FeeTypes
{
    public class CreateFeeTypeRequest
    {
        public string Name { get; set; } =
            string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } =
            true;
    }
}