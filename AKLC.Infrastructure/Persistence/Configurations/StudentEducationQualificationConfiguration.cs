using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentEducationQualificationConfiguration
        : IEntityTypeConfiguration<StudentEducationQualification>
    {
        public void Configure(
            EntityTypeBuilder<StudentEducationQualification> builder)
        {
            builder.ToTable("StudentEducationQualifications");

            builder.HasKey(x => x.Id);

            // =========================================
            // RELATIONSHIP
            // =========================================

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.HasOne(x => x.Student)
                .WithMany(x => x.EducationQualifications)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================
            // EDUCATION LEVEL
            // =========================================

            builder.Property(x => x.EducationLevel)
                .IsRequired();

            // One qualification per education level
            builder.HasIndex(x => new
            {
                x.StudentId,
                x.EducationLevel
            })
            .IsUnique();

            // =========================================
            // QUALIFICATION INFORMATION
            // =========================================

            builder.Property(x => x.ExaminationName)
                .HasMaxLength(150);

            builder.Property(x => x.PassingYear);

            builder.Property(x => x.Result)
                .HasMaxLength(50);

            builder.Property(x => x.BoardOrUniversity)
                .HasMaxLength(200);

            builder.Property(x => x.InstitutionName)
                .HasMaxLength(200);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);

            // =========================================
            // DISPLAY ORDER
            // =========================================

            builder.Property(x => x.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}