using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_NotificationTemplates_TemplateId",
                table: "UserNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "UserNotifications",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "UserNotifications",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "UserNotifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "InApp");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "UserNotifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "UserNotifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "UserNotifications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "General");

            migrationBuilder.AlterColumn<bool>(
                name: "PushEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "InAppEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "EmailEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "UserNotificationPreferences",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "RealTime");

            migrationBuilder.AddColumn<bool>(
                name: "QuietHoursEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "NotificationTemplates",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "NotificationTemplates",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "InApp");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "NotificationTemplates",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "NotificationLogs",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Sent"),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_NotificationLogs_UserNotifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "UserNotifications",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ReferenceId",
                table: "UserNotifications",
                column: "ReferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_Type",
                table: "UserNotifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationLogs_NotificationId",
                table: "NotificationLogs",
                column: "NotificationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_NotificationTemplates_TemplateId",
                table: "UserNotifications",
                column: "TemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_NotificationTemplates_TemplateId",
                table: "UserNotifications");

            migrationBuilder.DropTable(
                name: "NotificationLogs");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_ReferenceId",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_Type",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_UserId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "UserNotificationPreferences");

            migrationBuilder.DropColumn(
                name: "QuietHoursEnabled",
                table: "UserNotificationPreferences");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "NotificationTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "UserNotifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "UserNotifications",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "PushEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "InAppEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<bool>(
                name: "EmailEnabled",
                table: "UserNotificationPreferences",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "NotificationTemplates",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_AspNetUsers_UserId",
                table: "UserNotifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_NotificationTemplates_TemplateId",
                table: "UserNotifications",
                column: "TemplateId",
                principalTable: "NotificationTemplates",
                principalColumn: "TemplateId");
        }
    }
}
