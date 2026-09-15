using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class VideoConfiguration
        : IEntityTypeConfiguration<Video>
    {
        public void Configure(
            EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Videos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.SourceType)
                .IsRequired();

            builder.Property(x => x.VideoPath)
                .HasMaxLength(500);

            builder.Property(x => x.VideoUrl)
                .HasMaxLength(1000);

            builder.Property(x => x.ThumbnailPath)
                .HasMaxLength(500);

            builder.Property(x => x.HasCustomThumbnail)
                .IsRequired();

            builder.Property(x => x.DisplayOrder)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.IsDeleted)
                .IsRequired();

            builder.HasIndex(x => x.IsActive);

            builder.HasIndex(x => x.IsDeleted);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}