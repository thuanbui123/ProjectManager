using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace INFRASTRUCTURE.Migrations
{
    /// <inheritdoc />
    public partial class renameEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshTokenEntity_Users_UserId",
                table: "UserRefreshTokenEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokenEntity",
                table: "UserRefreshTokenEntity");

            migrationBuilder.RenameTable(
                name: "UserRefreshTokenEntity",
                newName: "UserRefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshTokenEntity_UserId",
                table: "UserRefreshToken",
                newName: "IX_UserRefreshToken_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshToken",
                table: "UserRefreshToken",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshToken_Users_UserId",
                table: "UserRefreshToken",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRefreshToken_Users_UserId",
                table: "UserRefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshToken",
                table: "UserRefreshToken");

            migrationBuilder.RenameTable(
                name: "UserRefreshToken",
                newName: "UserRefreshTokenEntity");

            migrationBuilder.RenameIndex(
                name: "IX_UserRefreshToken_UserId",
                table: "UserRefreshTokenEntity",
                newName: "IX_UserRefreshTokenEntity_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokenEntity",
                table: "UserRefreshTokenEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRefreshTokenEntity_Users_UserId",
                table: "UserRefreshTokenEntity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
