using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnalyticsSnapshots_AspNetUsers_UserId",
                table: "UserAnalyticsSnapshots");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotType",
                table: "UserAnalyticsSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GeneratedAt",
                table: "UserAnalyticsSnapshots",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotType",
                table: "PlatformAnalyticsSnapshots",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GeneratedAt",
                table: "PlatformAnalyticsSnapshots",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PlatformAnalyticsEvents",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "PlatformAnalyticsEvents",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "ReportRequests",
                columns: table => new
                {
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "CSV"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRequests", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_ReportRequests_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAnalyticsSnapshots_SnapshotType_GeneratedAt",
                table: "PlatformAnalyticsSnapshots",
                columns: new[] { "SnapshotType", "GeneratedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformAnalyticsEvents_UserId",
                table: "PlatformAnalyticsEvents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_RequestedByUserId",
                table: "ReportRequests",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_Status",
                table: "ReportRequests",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnalyticsSnapshots_AspNetUsers_UserId",
                table: "UserAnalyticsSnapshots",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnalyticsSnapshots_AspNetUsers_UserId",
                table: "UserAnalyticsSnapshots");

            migrationBuilder.DropTable(
                name: "ReportRequests");

            migrationBuilder.DropIndex(
                name: "IX_PlatformAnalyticsSnapshots_SnapshotType_GeneratedAt",
                table: "PlatformAnalyticsSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_PlatformAnalyticsEvents_UserId",
                table: "PlatformAnalyticsEvents");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotType",
                table: "UserAnalyticsSnapshots",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GeneratedAt",
                table: "UserAnalyticsSnapshots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "SnapshotType",
                table: "PlatformAnalyticsSnapshots",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GeneratedAt",
                table: "PlatformAnalyticsSnapshots",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "PlatformAnalyticsEvents",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "PlatformAnalyticsEvents",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnalyticsSnapshots_AspNetUsers_UserId",
                table: "UserAnalyticsSnapshots",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
