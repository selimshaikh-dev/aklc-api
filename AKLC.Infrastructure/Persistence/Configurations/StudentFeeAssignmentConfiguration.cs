using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentFeeAssignmentConfiguration
        : IEntityTypeConfiguration<StudentFeeAssignment>
    {
        public void Configure(
            EntityTypeBuilder<StudentFeeAssignment> builder)
        {
            builder.ToTable("StudentFeeAssignments");

            builder.HasKey(x => x.Id);

            // =========================================
            // AMOUNTS
            // =========================================

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.DiscountAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.NetAmount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            // =========================================
            // STUDENT RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.Student)
                .WithMany(x => x.FeeAssignments)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // FEE TYPE RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.FeeType)
                .WithMany(x => x.StudentFeeAssignments)
                .HasForeignKey(x => x.FeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // UNIQUE STUDENT + FEE TYPE
            // =========================================

            builder.HasIndex(x => new
            {
                x.StudentId,
                x.FeeTypeId
            })
            .IsUnique();
        }
    }
}