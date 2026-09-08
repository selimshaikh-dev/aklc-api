using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class CourseFeeStructureConfiguration
        : IEntityTypeConfiguration<CourseFeeStructure>
    {
        public void Configure(
            EntityTypeBuilder<CourseFeeStructure> builder)
        {
            builder.ToTable("CourseFeeStructures");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DefaultAmount)
                .HasPrecision(18, 2);

            // =========================================
            // COURSE RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.Course)
                .WithMany(x => x.FeeStructures)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // FEE TYPE RELATIONSHIP
            // =========================================

            builder.HasOne(x => x.FeeType)
                .WithMany(x => x.CourseFeeStructures)
                .HasForeignKey(x => x.FeeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================================
            // UNIQUE COURSE + FEE TYPE
            // =========================================

            builder.HasIndex(x => new
            {
                x.CourseId,
                x.FeeTypeId
            })
            .IsUnique();
        }
    }
}