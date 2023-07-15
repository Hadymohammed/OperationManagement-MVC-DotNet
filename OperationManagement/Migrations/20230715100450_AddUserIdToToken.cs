using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Tokens",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_userId",
                table: "Tokens",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Users_userId",
                table: "Tokens",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Users_userId",
                table: "Tokens");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_userId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Tokens");
        }
    }
}
