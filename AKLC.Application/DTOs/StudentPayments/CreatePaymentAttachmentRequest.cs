namespace AKLC.Application.DTOs.StudentPayments
{
    public class CreatePaymentAttachmentRequest
    {
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
    }
}