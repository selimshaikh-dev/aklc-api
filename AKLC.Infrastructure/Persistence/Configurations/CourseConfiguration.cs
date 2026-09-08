using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration
        : IEntityTypeConfiguration<Course>
    {
        public void Configure(
            EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Code)
                .HasMaxLength(50);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Code);
        }
    }
}