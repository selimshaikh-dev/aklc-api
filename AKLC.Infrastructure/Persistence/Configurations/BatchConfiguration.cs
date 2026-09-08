using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class BatchConfiguration
        : IEntityTypeConfiguration<Batch>
    {
        public void Configure(
            EntityTypeBuilder<Batch> builder)
        {
            builder.ToTable("Batches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Code)
                .HasMaxLength(50);

            builder.Property(x => x.StartDate);

            builder.Property(x => x.EndDate);

            builder.HasOne(x => x.Course)
                .WithMany(x => x.Batches)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}