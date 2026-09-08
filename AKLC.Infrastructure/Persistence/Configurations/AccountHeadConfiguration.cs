using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class AccountHeadConfiguration
        : IEntityTypeConfiguration<AccountHead>
    {
        public void Configure(
            EntityTypeBuilder<AccountHead> builder)
        {
            // =========================================
            // TABLE
            // =========================================

            builder.ToTable(
                "AccountHeads");


            // =========================================
            // PRIMARY KEY
            // =========================================

            builder.HasKey(
                x => x.Id);


            // =========================================
            // NAME
            // =========================================

            builder.Property(
                    x => x.Name)
                .IsRequired()
                .HasMaxLength(150);


            // =========================================
            // TYPE
            // Income / Expense
            // =========================================

            builder.Property(
                    x => x.Type)
                .IsRequired()
                .HasMaxLength(20);


            // =========================================
            // STATUS
            // =========================================

            builder.Property(
                    x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);


            builder.Property(
                    x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);


            // =========================================
            // CREATED AT
            // =========================================

            builder.Property(
                    x => x.CreatedAt)
                .IsRequired();


            // =========================================
            // UNIQUE ACCOUNT HEAD
            // Same name cannot be duplicated
            // inside the same account type.
            //
            // Example:
            //
            // Income + Admission Fee = unique
            // Expense + Admission Fee = allowed
            // =========================================

            builder.HasIndex(
                    x => new
                    {
                        x.Name,
                        x.Type
                    })
                .IsUnique();


            // =========================================
            // INDEXES
            // =========================================

            builder.HasIndex(
                x => x.Type);


            builder.HasIndex(
                x => x.IsActive);


            builder.HasIndex(
                x => x.IsDeleted);
        }
    }
}