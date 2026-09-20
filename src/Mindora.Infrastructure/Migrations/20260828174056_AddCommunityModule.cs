using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityComments_AspNetUsers_UserId",
                table: "CommunityComments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroupMembers_AspNetUsers_UserId",
                table: "CommunityGroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroups_AspNetUsers_CreatedByUserId",
                table: "CommunityGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_AspNetUsers_ReportedByUserId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_CommunityComments_CommentId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_CommunityPosts_PostId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityPosts_AspNetUsers_UserId",
                table: "CommunityPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityPosts_CommunityGroups_GroupId",
                table: "CommunityPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_AspNetUsers_UserId",
                table: "CommunityReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_CommunityComments_CommentId",
                table: "CommunityReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_CommunityPosts_PostId",
                table: "CommunityReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommunityReactions_UserId",
                table: "CommunityReactions");

            migrationBuilder.AlterColumn<string>(
                name: "ReactionType",
                table: "CommunityReactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityReactions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CommunityPosts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAnonymous",
                table: "CommunityPosts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityPosts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CommunityPosts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "CommunityPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CommunityModerationFlags",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "CommunityModerationFlags",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityModerationFlags",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CommunityGroups",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CommunityGroups",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityGroups",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "CommunityGroupMembers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityComments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "CommunityComments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityReactions_UserId_CommentId",
                table: "CommunityReactions",
                columns: new[] { "UserId", "CommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityReactions_UserId_PostId",
                table: "CommunityReactions",
                columns: new[] { "UserId", "PostId" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunityPosts_IsPinned",
                table: "CommunityPosts",
                column: "IsPinned");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityGroups_Name",
                table: "CommunityGroups",
                column: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityComments_AspNetUsers_UserId",
                table: "CommunityComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroupMembers_AspNetUsers_UserId",
                table: "CommunityGroupMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroups_AspNetUsers_CreatedByUserId",
                table: "CommunityGroups",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_AspNetUsers_ReportedByUserId",
                table: "CommunityModerationFlags",
                column: "ReportedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_CommunityComments_CommentId",
                table: "CommunityModerationFlags",
                column: "CommentId",
                principalTable: "CommunityComments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_CommunityPosts_PostId",
                table: "CommunityModerationFlags",
                column: "PostId",
                principalTable: "CommunityPosts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityPosts_AspNetUsers_UserId",
                table: "CommunityPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityPosts_CommunityGroups_GroupId",
                table: "CommunityPosts",
                column: "GroupId",
                principalTable: "CommunityGroups",
                principalColumn: "GroupId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_AspNetUsers_UserId",
                table: "CommunityReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_CommunityComments_CommentId",
                table: "CommunityReactions",
                column: "CommentId",
                principalTable: "CommunityComments",
                principalColumn: "CommentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_CommunityPosts_PostId",
                table: "CommunityReactions",
                column: "PostId",
                principalTable: "CommunityPosts",
                principalColumn: "PostId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommunityComments_AspNetUsers_UserId",
                table: "CommunityComments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroupMembers_AspNetUsers_UserId",
                table: "CommunityGroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityGroups_AspNetUsers_CreatedByUserId",
                table: "CommunityGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_AspNetUsers_ReportedByUserId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_CommunityComments_CommentId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityModerationFlags_CommunityPosts_PostId",
                table: "CommunityModerationFlags");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityPosts_AspNetUsers_UserId",
                table: "CommunityPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityPosts_CommunityGroups_GroupId",
                table: "CommunityPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_AspNetUsers_UserId",
                table: "CommunityReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_CommunityComments_CommentId",
                table: "CommunityReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityReactions_CommunityPosts_PostId",
                table: "CommunityReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommunityReactions_UserId_CommentId",
                table: "CommunityReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommunityReactions_UserId_PostId",
                table: "CommunityReactions");

            migrationBuilder.DropIndex(
                name: "IX_CommunityPosts_IsPinned",
                table: "CommunityPosts");

            migrationBuilder.DropIndex(
                name: "IX_CommunityGroups_Name",
                table: "CommunityGroups");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CommunityPosts");

            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "CommunityPosts");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CommunityGroups");

            migrationBuilder.AlterColumn<string>(
                name: "ReactionType",
                table: "CommunityReactions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityReactions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "CommunityPosts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAnonymous",
                table: "CommunityPosts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityPosts",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CommunityModerationFlags",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "Pending");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "CommunityModerationFlags",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityModerationFlags",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CommunityGroups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "CommunityGroups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "CommunityGroupMembers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "CommunityComments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "CommunityComments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityReactions_UserId",
                table: "CommunityReactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityComments_AspNetUsers_UserId",
                table: "CommunityComments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroupMembers_AspNetUsers_UserId",
                table: "CommunityGroupMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityGroups_AspNetUsers_CreatedByUserId",
                table: "CommunityGroups",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_AspNetUsers_ReportedByUserId",
                table: "CommunityModerationFlags",
                column: "ReportedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_CommunityComments_CommentId",
                table: "CommunityModerationFlags",
                column: "CommentId",
                principalTable: "CommunityComments",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityModerationFlags_CommunityPosts_PostId",
                table: "CommunityModerationFlags",
                column: "PostId",
                principalTable: "CommunityPosts",
                principalColumn: "PostId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityPosts_AspNetUsers_UserId",
                table: "CommunityPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityPosts_CommunityGroups_GroupId",
                table: "CommunityPosts",
                column: "GroupId",
                principalTable: "CommunityGroups",
                principalColumn: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_AspNetUsers_UserId",
                table: "CommunityReactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_CommunityComments_CommentId",
                table: "CommunityReactions",
                column: "CommentId",
                principalTable: "CommunityComments",
                principalColumn: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityReactions_CommunityPosts_PostId",
                table: "CommunityReactions",
                column: "PostId",
                principalTable: "CommunityPosts",
                principalColumn: "PostId");
        }
    }
}
