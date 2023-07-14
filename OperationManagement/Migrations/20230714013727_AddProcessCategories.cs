using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Processes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProcessCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoramlizedName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Processes_CategoryId",
                table: "Processes",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Processes_ProcessCategories_CategoryId",
                table: "Processes",
                column: "CategoryId",
                principalTable: "ProcessCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Processes_ProcessCategories_CategoryId",
                table: "Processes");

            migrationBuilder.DropTable(
                name: "ProcessCategories");

            migrationBuilder.DropIndex(
                name: "IX_Processes_CategoryId",
                table: "Processes");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Processes");
        }
    }
}
