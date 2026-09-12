using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class StudentConfiguration
        : IEntityTypeConfiguration<Student>
    {
        public void Configure(
            EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");

            builder.HasKey(x => x.Id);

            // =========================================
            // STUDENT CODE
            // =========================================

            builder.Property(x => x.StudentCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasIndex(x => x.StudentCode)
                .IsUnique();

            // =========================================
            // BASIC INFORMATION
            // =========================================

            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.MobileNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.MobileNumber);

            builder.Property(x => x.GuardianName)
                .HasMaxLength(150);

            builder.Property(x => x.GuardianMobile)
                .HasMaxLength(20);

            builder.Property(x => x.Email)
                .HasMaxLength(150);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.Gender)
                .HasMaxLength(20);

            builder.Property(x => x.AdmissionDate)
                .IsRequired();

            builder.Property(x => x.PhotoPath)
                .HasMaxLength(500);
        }
    }
}