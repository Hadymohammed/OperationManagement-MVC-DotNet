using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Components",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComponentCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnterpriseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComponentCategories_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Components_CategoryId",
                table: "Components",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentCategories_EnterpriseId",
                table: "ComponentCategories",
                column: "EnterpriseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_ComponentCategories_CategoryId",
                table: "Components",
                column: "CategoryId",
                principalTable: "ComponentCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_ComponentCategories_CategoryId",
                table: "Components");

            migrationBuilder.DropTable(
                name: "ComponentCategories");

            migrationBuilder.DropIndex(
                name: "IX_Components_CategoryId",
                table: "Components");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Components");
        }
    }
}
