using AKLC.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AKLC.Infrastructure.Persistence.Configurations
{
    public class ExpenseTransactionConfiguration
        : IEntityTypeConfiguration<ExpenseTransaction>
    {
        public void Configure(
            EntityTypeBuilder<ExpenseTransaction> builder)
        {
            // =========================================
            // TABLE
            // =========================================

            builder.ToTable(
                "ExpenseTransactions");


            // =========================================
            // PRIMARY KEY
            // =========================================

            builder.HasKey(
                x => x.Id);


            // =========================================
            // VOUCHER NUMBER
            // =========================================

            builder.Property(
                    x => x.VoucherNumber)
                .IsRequired()
                .HasMaxLength(50);


            builder.HasIndex(
                    x => x.VoucherNumber)
                .IsUnique();


            // =========================================
            // ACCOUNT HEAD
            // =========================================

            builder.Property(
                    x => x.AccountHeadId)
                .IsRequired();


            // =========================================
            // AMOUNT
            // =========================================

            builder.Property(
                    x => x.Amount)
                .HasPrecision(
                    18,
                    2)
                .IsRequired();


            // =========================================
            // TRANSACTION DATE
            // =========================================

            builder.Property(
                    x => x.TransactionDate)
                .IsRequired();


            // =========================================
            // PAYMENT METHOD
            // =========================================

            builder.Property(
                    x => x.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);


            // =========================================
            // REFERENCE NUMBER
            // =========================================

            builder.Property(
                    x => x.ReferenceNumber)
                .HasMaxLength(150);


            // =========================================
            // ATTACHMENT PATH
            // =========================================

            builder.Property(
                    x => x.AttachmentPath)
                .HasMaxLength(500);


            // =========================================
            // REMARKS
            // =========================================

            builder.Property(
                    x => x.Remarks)
                .HasMaxLength(1000);


            // =========================================
            // CREATED BY
            // =========================================

            builder.Property(
                x => x.CreatedBy);


            // =========================================
            // CREATED AT
            // =========================================

            builder.Property(
                    x => x.CreatedAt)
                .IsRequired();


            // =========================================
            // SOFT DELETE
            // =========================================

            builder.Property(
                    x => x.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);


            // =========================================
            // RELATIONSHIP
            // =========================================

            builder
                .HasOne(
                    x => x.AccountHead)
                .WithMany(
                    x => x.ExpenseTransactions)
                .HasForeignKey(
                    x => x.AccountHeadId)
                .OnDelete(
                    DeleteBehavior.Restrict);


            // =========================================
            // INDEXES
            // =========================================

            builder.HasIndex(
                x => x.AccountHeadId);


            builder.HasIndex(
                x => x.TransactionDate);


            builder.HasIndex(
                x => x.PaymentMethod);


            builder.HasIndex(
                x => x.IsDeleted);


            builder.HasIndex(
                x => new
                {
                    x.TransactionDate,
                    x.IsDeleted
                });
        }
    }
}