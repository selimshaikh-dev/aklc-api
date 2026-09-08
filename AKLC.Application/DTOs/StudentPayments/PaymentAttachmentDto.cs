namespace AKLC.Application.DTOs.StudentPayments
{
    public class PaymentAttachmentDto
    {
        public Guid Id { get; set; }

        public string OriginalFileName { get; set; }
            = string.Empty;

        public string StoredFileName { get; set; }
            = string.Empty;

        public string FilePath { get; set; }
            = string.Empty;

        public string ContentType { get; set; }
            = string.Empty;

        public long FileSize { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}