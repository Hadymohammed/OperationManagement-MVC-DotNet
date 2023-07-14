using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessCategoriesToEnterprise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnterpriseId",
                table: "ProcessCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessCategories_EnterpriseId",
                table: "ProcessCategories",
                column: "EnterpriseId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessCategories_Enterprises_EnterpriseId",
                table: "ProcessCategories",
                column: "EnterpriseId",
                principalTable: "Enterprises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessCategories_Enterprises_EnterpriseId",
                table: "ProcessCategories");

            migrationBuilder.DropIndex(
                name: "IX_ProcessCategories_EnterpriseId",
                table: "ProcessCategories");

            migrationBuilder.DropColumn(
                name: "EnterpriseId",
                table: "ProcessCategories");
        }
    }
}
