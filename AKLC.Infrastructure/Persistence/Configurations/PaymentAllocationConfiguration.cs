using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class PaymentAllocationConfiguration
        : IEntityTypeConfiguration<PaymentAllocation>
    {
        public void Configure(
            EntityTypeBuilder<PaymentAllocation> builder)
        {
            builder.ToTable("PaymentAllocations");

            builder.HasKey(x => x.Id);

            // =========================================
            // AMOUNT
            // =========================================

            builder.Property(x => x.AllocatedAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            // =========================================
            // STUDENT PAYMENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.StudentPayment)
                .WithMany(x => x.Allocations)
                .HasForeignKey(x => x.StudentPaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // PAYMENT SCHEDULE RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.PaymentSchedule)
                .WithMany(x => x.Allocations)
                .HasForeignKey(x => x.PaymentScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}