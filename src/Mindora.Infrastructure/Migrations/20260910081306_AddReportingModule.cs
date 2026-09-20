using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReportingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReportType",
                table: "ReportRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "TemplateId",
                table: "ReportRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReportTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Business"),
                    DefaultFormat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "CSV"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    GeneratedReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "CSV"),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.GeneratedReportId);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_AspNetUsers_RequestedByUserId",
                        column: x => x.RequestedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_ReportTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ReportTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledReports",
                columns: table => new
                {
                    ScheduledReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Format = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "CSV"),
                    Frequency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Daily"),
                    ScheduledTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    LastRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextRunAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledReports", x => x.ScheduledReportId);
                    table.ForeignKey(
                        name: "FK_ScheduledReports_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScheduledReports_ReportTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ReportTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReportRecipients",
                columns: table => new
                {
                    ReportRecipientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryChannel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Email"),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportRecipients", x => x.ReportRecipientId);
                    table.ForeignKey(
                        name: "FK_ReportRecipients_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportRecipients_ScheduledReports_ScheduledReportId",
                        column: x => x.ScheduledReportId,
                        principalTable: "ScheduledReports",
                        principalColumn: "ScheduledReportId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_ReportType_RequestedAt",
                table: "ReportRequests",
                columns: new[] { "ReportType", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportRequests_TemplateId",
                table: "ReportRequests",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ReportType_RequestedAt",
                table: "GeneratedReports",
                columns: new[] { "ReportType", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_RequestedByUserId",
                table: "GeneratedReports",
                column: "RequestedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_Status",
                table: "GeneratedReports",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_TemplateId",
                table: "GeneratedReports",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportRecipients_ScheduledReportId_UserId",
                table: "ReportRecipients",
                columns: new[] { "ScheduledReportId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportRecipients_UserId",
                table: "ReportRecipients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_Category",
                table: "ReportTemplates",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplates_TemplateKey",
                table: "ReportTemplates",
                column: "TemplateKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledReports_CreatedByUserId",
                table: "ScheduledReports",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledReports_IsActive",
                table: "ScheduledReports",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledReports_NextRunAt",
                table: "ScheduledReports",
                column: "NextRunAt");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledReports_TemplateId",
                table: "ScheduledReports",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportRequests_ReportTemplates_TemplateId",
                table: "ReportRequests",
                column: "TemplateId",
                principalTable: "ReportTemplates",
                principalColumn: "TemplateId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportRequests_ReportTemplates_TemplateId",
                table: "ReportRequests");

            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "ReportRecipients");

            migrationBuilder.DropTable(
                name: "ScheduledReports");

            migrationBuilder.DropTable(
                name: "ReportTemplates");

            migrationBuilder.DropIndex(
                name: "IX_ReportRequests_ReportType_RequestedAt",
                table: "ReportRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReportRequests_TemplateId",
                table: "ReportRequests");

            migrationBuilder.DropColumn(
                name: "TemplateId",
                table: "ReportRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ReportType",
                table: "ReportRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
