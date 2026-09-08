using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration
        : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(
            EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(x => x.Id);

            // =========================================
            // ACTION
            // =========================================

            builder.Property(x => x.Action)
                .IsRequired()
                .HasMaxLength(100);

            // =========================================
            // ENTITY INFORMATION
            // =========================================

            builder.Property(x => x.EntityName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.EntityId)
                .HasMaxLength(100);

            // =========================================
            // DESCRIPTION / VALUES
            // =========================================

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(x => x.NewValues)
                .HasColumnType("nvarchar(max)");

            // =========================================
            // REQUEST INFORMATION
            // =========================================

            builder.Property(x => x.IpAddress)
                .HasMaxLength(100);

            builder.Property(x => x.UserAgent)
                .HasMaxLength(1000);

            // =========================================
            // INDEXES
            // =========================================

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.Action);

            builder.HasIndex(x => new
            {
                x.EntityName,
                x.EntityId
            });

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}