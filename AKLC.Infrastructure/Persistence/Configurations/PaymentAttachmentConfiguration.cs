using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class PaymentAttachmentConfiguration
        : IEntityTypeConfiguration<PaymentAttachment>
    {
        public void Configure(
            EntityTypeBuilder<PaymentAttachment> builder)
        {
            builder.ToTable("PaymentAttachments");

            builder.HasKey(x => x.Id);

            // =========================================
            // FILE INFORMATION
            // =========================================

            builder.Property(x => x.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.StoredFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(x => x.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.ContentType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.FileSize)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            // =========================================
            // STUDENT PAYMENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.StudentPayment)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.StudentPaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}