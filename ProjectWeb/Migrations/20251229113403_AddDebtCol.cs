using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectWeb.Migrations
{
    /// <inheritdoc />
    public partial class AddDebtCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DebtNotifiedMonth",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsInDebt",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DebtNotifiedMonth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsInDebt",
                table: "AspNetUsers");
        }
    }
}
