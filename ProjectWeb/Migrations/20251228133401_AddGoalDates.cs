using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddGoalDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AchievedDate",
                table: "Goals",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievedDate",
                table: "Goals");
        }
    }
}
