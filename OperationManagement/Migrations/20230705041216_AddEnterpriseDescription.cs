using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Enterprises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Enterprises");
        }
    }
}
