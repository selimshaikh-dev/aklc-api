using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class FeeTypeConfiguration
        : IEntityTypeConfiguration<FeeType>
    {
        public void Configure(
            EntityTypeBuilder<FeeType> builder)
        {
            builder.ToTable("FeeTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}