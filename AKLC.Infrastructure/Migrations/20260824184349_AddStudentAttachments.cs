using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentProofPath",
                table: "Students",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Students",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentProofPath",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Students");
        }
    }
}
