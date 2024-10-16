using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialApp.Migrations
{
    /// <inheritdoc />
    public partial class removedNavigationProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostModelId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Users_UserModelId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Users_UserModelId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserModelId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PostModelId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UserModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PostModelId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PostModelId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserModelId",
                table: "Comments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserModelId",
                table: "Posts",
                column: "UserModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostModelId",
                table: "Comments",
                column: "PostModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserModelId",
                table: "Comments",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostModelId",
                table: "Comments",
                column: "PostModelId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Users_UserModelId",
                table: "Comments",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Users_UserModelId",
                table: "Posts",
                column: "UserModelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
