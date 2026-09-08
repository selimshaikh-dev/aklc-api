using AKLC.Domain.Constants;
using AKLC.Domain.Entities;
using AKLC.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.Property(x => x.CreatedAt)
            .IsRequired();


        // =========================
        // Seed Roles
        // =========================

        builder.HasData(

            new Role
            {
                Id = SeedData.AdminRoleId,
                Name = AppRoles.Admin,
                Description = "System Administrator",
                CreatedAt = new DateTime(
                    2026, 1, 1,
                    0, 0, 0,
                    DateTimeKind.Utc)
            },

            new Role
            {
                Id = SeedData.TeacherRoleId,
                Name = AppRoles.Teacher,
                Description = "Teacher",
                CreatedAt = new DateTime(
                    2026, 1, 1,
                    0, 0, 0,
                    DateTimeKind.Utc)
            },

            new Role
            {
                Id = SeedData.StudentRoleId,
                Name = AppRoles.Student,
                Description = "Student",
                CreatedAt = new DateTime(
                    2026, 1, 1,
                    0, 0, 0,
                    DateTimeKind.Utc)
            }

        );
    }
}