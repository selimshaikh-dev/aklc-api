using AKLC.Domain.Common;

namespace AKLC.Domain.Entities
{
    public class PaymentAttachment : BaseEntity
    {
        public Guid StudentPaymentId { get; set; }

        // User uploaded filename
        public string OriginalFileName { get; set; } = string.Empty;

        // System generated unique filename
        public string StoredFileName { get; set; } = string.Empty;

        // Example:
        // /uploads/payments/2026/08/file.pdf
        public string FilePath { get; set; } = string.Empty;

        // image/jpeg
        // image/png
        // application/pdf
        public string ContentType { get; set; } = string.Empty;

        // Bytes
        public long FileSize { get; set; }

        public string? Description { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation
        public StudentPayment StudentPayment { get; set; } = null!;
    }
}