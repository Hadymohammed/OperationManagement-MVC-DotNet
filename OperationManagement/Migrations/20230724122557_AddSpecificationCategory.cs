using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecificationCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Specifications",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SpecificationCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoramlizedName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EnterpriseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificationCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecificationCategories_Enterprises_EnterpriseId",
                        column: x => x.EnterpriseId,
                        principalTable: "Enterprises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Specifications_CategoryId",
                table: "Specifications",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificationCategories_EnterpriseId",
                table: "SpecificationCategories",
                column: "EnterpriseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Specifications_SpecificationCategories_CategoryId",
                table: "Specifications",
                column: "CategoryId",
                principalTable: "SpecificationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Specifications_SpecificationCategories_CategoryId",
                table: "Specifications");

            migrationBuilder.DropTable(
                name: "SpecificationCategories");

            migrationBuilder.DropIndex(
                name: "IX_Specifications_CategoryId",
                table: "Specifications");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Specifications");
        }
    }
}
