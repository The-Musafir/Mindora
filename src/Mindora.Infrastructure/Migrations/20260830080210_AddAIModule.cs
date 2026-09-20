using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAIModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIWellnessCoachSessions_AspNetUsers_UserId",
                table: "AIWellnessCoachSessions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "AIWellnessCoachSessions",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AIWellnessCoachSessions",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "LastContext",
                table: "AIWellnessCoachSessions",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "AIChatMessages",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Sender",
                table: "AIChatMessages",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "User",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "AIChatMessages",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Intent",
                table: "AIChatMessages",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MoodScore",
                table: "AIChatMessages",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "AIChatMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sentiment",
                table: "AIChatMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AIFeedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIFeedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_AIFeedbacks_AIChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "AIChatMessages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIFeedbacks_AIWellnessCoachSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "AIWellnessCoachSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AIFeedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIPromptTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateKey = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PromptText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Supportive"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIPromptTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "RiskPredictions",
                columns: table => new
                {
                    RiskPredictionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Low"),
                    Probability = table.Column<double>(type: "float", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PredictedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiskPredictions", x => x.RiskPredictionId);
                    table.ForeignKey(
                        name: "FK_RiskPredictions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WellnessScores",
                columns: table => new
                {
                    WellnessScoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WellnessScores", x => x.WellnessScoreId);
                    table.ForeignKey(
                        name: "FK_WellnessScores_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AIInteractionLogs",
                columns: table => new
                {
                    LogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIInteractionLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_AIInteractionLogs_AIPromptTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "AIPromptTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AIInteractionLogs_AIWellnessCoachSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "AIWellnessCoachSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIWellnessCoachSessions_IsActive",
                table: "AIWellnessCoachSessions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AIChatMessages_SessionId",
                table: "AIChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AIFeedbacks_MessageId",
                table: "AIFeedbacks",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AIFeedbacks_SessionId_UserId_MessageId",
                table: "AIFeedbacks",
                columns: new[] { "SessionId", "UserId", "MessageId" });

            migrationBuilder.CreateIndex(
                name: "IX_AIFeedbacks_UserId",
                table: "AIFeedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractionLogs_SessionId",
                table: "AIInteractionLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AIInteractionLogs_TemplateId",
                table: "AIInteractionLogs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AIPromptTemplates_TemplateKey",
                table: "AIPromptTemplates",
                column: "TemplateKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskPredictions_UserId_PredictedAt",
                table: "RiskPredictions",
                columns: new[] { "UserId", "PredictedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_WellnessScores_UserId_CalculatedAt",
                table: "WellnessScores",
                columns: new[] { "UserId", "CalculatedAt" });

            migrationBuilder.AddForeignKey(
                name: "FK_AIWellnessCoachSessions_AspNetUsers_UserId",
                table: "AIWellnessCoachSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIWellnessCoachSessions_AspNetUsers_UserId",
                table: "AIWellnessCoachSessions");

            migrationBuilder.DropTable(
                name: "AIFeedbacks");

            migrationBuilder.DropTable(
                name: "AIInteractionLogs");

            migrationBuilder.DropTable(
                name: "RiskPredictions");

            migrationBuilder.DropTable(
                name: "WellnessScores");

            migrationBuilder.DropTable(
                name: "AIPromptTemplates");

            migrationBuilder.DropIndex(
                name: "IX_AIWellnessCoachSessions_IsActive",
                table: "AIWellnessCoachSessions");

            migrationBuilder.DropIndex(
                name: "IX_AIChatMessages_SessionId",
                table: "AIChatMessages");

            migrationBuilder.DropColumn(
                name: "LastContext",
                table: "AIWellnessCoachSessions");

            migrationBuilder.DropColumn(
                name: "Intent",
                table: "AIChatMessages");

            migrationBuilder.DropColumn(
                name: "MoodScore",
                table: "AIChatMessages");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "AIChatMessages");

            migrationBuilder.DropColumn(
                name: "Sentiment",
                table: "AIChatMessages");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "AIWellnessCoachSessions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AIWellnessCoachSessions",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentAt",
                table: "AIChatMessages",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Sender",
                table: "AIChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Metadata",
                table: "AIChatMessages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AIWellnessCoachSessions_AspNetUsers_UserId",
                table: "AIWellnessCoachSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
