using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConsultationModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsultationSessions",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Chat"),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false, defaultValue: "Scheduled"),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationSessions", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationSessions_ProfessionalProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProfessionalProviders",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationFeedbacks",
                columns: table => new
                {
                    FeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationFeedbacks", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_ConsultationFeedbacks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsultationFeedbacks_ConsultationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationNotes",
                columns: table => new
                {
                    NoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationNotes", x => x.NoteId);
                    table.ForeignKey(
                        name: "FK_ConsultationNotes_ConsultationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultationNotes_ProfessionalProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProfessionalProviders",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationPrescriptions",
                columns: table => new
                {
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationPrescriptions", x => x.PrescriptionId);
                    table.ForeignKey(
                        name: "FK_ConsultationPrescriptions_ConsultationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationReminders",
                columns: table => new
                {
                    ReminderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReminderAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "InApp"),
                    IsSent = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationReminders", x => x.ReminderId);
                    table.ForeignKey(
                        name: "FK_ConsultationReminders_ConsultationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FollowUpPlans",
                columns: table => new
                {
                    FollowUpPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowUpPlans", x => x.FollowUpPlanId);
                    table.ForeignKey(
                        name: "FK_FollowUpPlans_ConsultationSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ConsultationSessions",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationFeedbacks_SessionId_UserId",
                table: "ConsultationFeedbacks",
                columns: new[] { "SessionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationFeedbacks_UserId",
                table: "ConsultationFeedbacks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationNotes_ProviderId",
                table: "ConsultationNotes",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationNotes_SessionId",
                table: "ConsultationNotes",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationPrescriptions_SessionId",
                table: "ConsultationPrescriptions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationReminders_SessionId_ReminderAt",
                table: "ConsultationReminders",
                columns: new[] { "SessionId", "ReminderAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_AppointmentId",
                table: "ConsultationSessions",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_ProviderId",
                table: "ConsultationSessions",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_Status",
                table: "ConsultationSessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_UserId",
                table: "ConsultationSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultationSessions_UserId_ScheduledAt",
                table: "ConsultationSessions",
                columns: new[] { "UserId", "ScheduledAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FollowUpPlans_SessionId",
                table: "FollowUpPlans",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultationFeedbacks");

            migrationBuilder.DropTable(
                name: "ConsultationNotes");

            migrationBuilder.DropTable(
                name: "ConsultationPrescriptions");

            migrationBuilder.DropTable(
                name: "ConsultationReminders");

            migrationBuilder.DropTable(
                name: "FollowUpPlans");

            migrationBuilder.DropTable(
                name: "ConsultationSessions");
        }
    }
}
