using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDeleteAndPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroupMembers_Users_UserId",
                table: "CommunityGroupMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroupMembers_Users_UserId",
                table: "CommunityGroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroupMembers_Users_UserId",
                table: "CommunityGroupMembers");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroupMembers_Users_UserId",
                table: "CommunityGroupMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
