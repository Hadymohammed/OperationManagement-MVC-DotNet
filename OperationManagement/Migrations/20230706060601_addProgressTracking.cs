using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class addProgressTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Orders");
        }
    }
}
