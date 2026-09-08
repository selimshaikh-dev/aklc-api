using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class PaymentScheduleConfiguration
        : IEntityTypeConfiguration<PaymentSchedule>
    {
        public void Configure(
            EntityTypeBuilder<PaymentSchedule> builder)
        {
            builder.ToTable("PaymentSchedules");

            builder.HasKey(x => x.Id);

            // =========================================
            // BASIC INFORMATION
            // =========================================

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.ScheduledAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            // =========================================
            // STUDENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.Student)
                .WithMany(x => x.PaymentSchedules)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // FEE ASSIGNMENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.StudentFeeAssignment)
                .WithMany(x => x.PaymentSchedules)
                .HasForeignKey(x => x.StudentFeeAssignmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}