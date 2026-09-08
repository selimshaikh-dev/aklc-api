using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountHeads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHeads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseTransactions_AccountHeads_AccountHeadId",
                        column: x => x.AccountHeadId,
                        principalTable: "AccountHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomeTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoucherNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AccountHeadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeTransactions_AccountHeads_AccountHeadId",
                        column: x => x.AccountHeadId,
                        principalTable: "AccountHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeads_IsActive",
                table: "AccountHeads",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeads_IsDeleted",
                table: "AccountHeads",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeads_Name_Type",
                table: "AccountHeads",
                columns: new[] { "Name", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeads_Type",
                table: "AccountHeads",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_AccountHeadId",
                table: "ExpenseTransactions",
                column: "AccountHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_IsDeleted",
                table: "ExpenseTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_PaymentMethod",
                table: "ExpenseTransactions",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_TransactionDate",
                table: "ExpenseTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_TransactionDate_IsDeleted",
                table: "ExpenseTransactions",
                columns: new[] { "TransactionDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseTransactions_VoucherNumber",
                table: "ExpenseTransactions",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_AccountHeadId",
                table: "IncomeTransactions",
                column: "AccountHeadId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_IsDeleted",
                table: "IncomeTransactions",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_PaymentMethod",
                table: "IncomeTransactions",
                column: "PaymentMethod");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_TransactionDate",
                table: "IncomeTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_TransactionDate_IsDeleted",
                table: "IncomeTransactions",
                columns: new[] { "TransactionDate", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeTransactions_VoucherNumber",
                table: "IncomeTransactions",
                column: "VoucherNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseTransactions");

            migrationBuilder.DropTable(
                name: "IncomeTransactions");

            migrationBuilder.DropTable(
                name: "AccountHeads");
        }
    }
}
