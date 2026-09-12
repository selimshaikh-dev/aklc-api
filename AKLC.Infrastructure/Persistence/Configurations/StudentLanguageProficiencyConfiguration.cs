using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentLanguageProficiencyConfiguration
        : IEntityTypeConfiguration<StudentLanguageProficiency>
    {
        public void Configure(
            EntityTypeBuilder<StudentLanguageProficiency> builder)
        {
            builder.ToTable("StudentLanguageProficiencies");

            builder.HasKey(x => x.Id);

            // =========================================
            // RELATIONSHIP
            // =========================================

            builder.Property(x => x.StudentId)
                .IsRequired();

            builder.HasOne(x => x.Student)
                .WithMany(x => x.LanguageProficiencies)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================================
            // LANGUAGE INFORMATION
            // =========================================

            builder.Property(x => x.LanguageName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.ProficiencyLevel)
                .HasMaxLength(50);

            builder.Property(x => x.InstitutionName)
                .HasMaxLength(200);

            builder.Property(x => x.Remarks)
                .HasMaxLength(500);
        }
    }
}