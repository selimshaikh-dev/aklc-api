using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentProfessionalExperienceConfiguration
        : IEntityTypeConfiguration<StudentProfessionalExperience>
    {
        public void Configure(
            EntityTypeBuilder<StudentProfessionalExperience> builder)
        {
            builder.ToTable("StudentProfessionalExperiences");

            builder.HasKey(x => x.Id);

            // =========================================
            // RELATIONSHIP
            // =========================================

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.HasOne(x => x.Student)
                .WithMany(x => x.ProfessionalExperiences)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================
            // EXPERIENCE INFORMATION
            // =========================================

            builder.Property(x => x.ExperienceTitle)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.OrganizationName)
                .HasMaxLength(200);

            builder.Property(x => x.YearsOfExperience)
                .HasPrecision(5, 2);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);
        }
    }
}