using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AKLC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNextPaymentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "NextPaymentDate",
                table: "Students",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextPaymentDate",
                table: "Students");
        }
    }
}
