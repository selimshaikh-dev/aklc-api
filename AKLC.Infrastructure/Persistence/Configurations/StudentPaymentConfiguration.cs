using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentPaymentConfiguration
        : IEntityTypeConfiguration<StudentPayment>
    {
        public void Configure(
            EntityTypeBuilder<StudentPayment> builder)
        {
            builder.ToTable("StudentPayments");

            builder.HasKey(x => x.Id);

            // =========================================
            // RECEIPT
            // =========================================

            builder.Property(x => x.ReceiptNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.ReceiptNumber)
                .IsUnique();

            // =========================================
            // PAYMENT INFORMATION
            // =========================================

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.PaymentDate)
                .IsRequired();

            builder.Property(x => x.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(150);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            // =========================================
            // REVERSAL
            // =========================================

            builder.Property(x => x.ReversalReason)
                .HasMaxLength(500);

            builder.HasIndex(x => x.ReversedByUserId);

            // =========================================
            // STUDENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.Student)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}