using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalHabitModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HabitTrackingEntries");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Habits");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArchived",
                table: "Habits",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Habits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Habits",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Habits",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Habits",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "HabitCategoryId",
                table: "Habits",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Habits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HabitCategories",
                columns: table => new
                {
                    HabitCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitCategories", x => x.HabitCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "HabitLogs",
                columns: table => new
                {
                    HabitLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Reflection = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MoodBefore = table.Column<int>(type: "int", nullable: true, comment: "1-10 scale"),
                    MoodAfter = table.Column<int>(type: "int", nullable: true, comment: "1-10 scale"),
                    CompletionTimeMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitLogs", x => x.HabitLogId);
                    table.ForeignKey(
                        name: "FK_HabitLogs_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "HabitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HabitReminders",
                columns: table => new
                {
                    HabitReminderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReminderTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitReminders", x => x.HabitReminderId);
                    table.ForeignKey(
                        name: "FK_HabitReminders_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "HabitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecoveryStreaks",
                columns: table => new
                {
                    RecoveryStreakId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LongestStreak = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryStreaks", x => x.RecoveryStreakId);
                    table.ForeignKey(
                        name: "FK_RecoveryStreaks_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "HabitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RelapseLogs",
                columns: table => new
                {
                    RelapseLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RelapseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Note = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelapseLogs", x => x.RelapseLogId);
                    table.ForeignKey(
                        name: "FK_RelapseLogs_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "HabitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Habits_HabitCategoryId",
                table: "Habits",
                column: "HabitCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HabitCategories_Name",
                table: "HabitCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HabitLogs_HabitId_LogDate",
                table: "HabitLogs",
                columns: new[] { "HabitId", "LogDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HabitReminders_HabitId",
                table: "HabitReminders",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryStreaks_HabitId",
                table: "RecoveryStreaks",
                column: "HabitId");

            migrationBuilder.CreateIndex(
                name: "IX_RelapseLogs_HabitId",
                table: "RelapseLogs",
                column: "HabitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Habits_HabitCategories_HabitCategoryId",
                table: "Habits",
                column: "HabitCategoryId",
                principalTable: "HabitCategories",
                principalColumn: "HabitCategoryId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Habits_HabitCategories_HabitCategoryId",
                table: "Habits");

            migrationBuilder.DropTable(
                name: "HabitCategories");

            migrationBuilder.DropTable(
                name: "HabitLogs");

            migrationBuilder.DropTable(
                name: "HabitReminders");

            migrationBuilder.DropTable(
                name: "RecoveryStreaks");

            migrationBuilder.DropTable(
                name: "RelapseLogs");

            migrationBuilder.DropIndex(
                name: "IX_Habits_HabitCategoryId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "HabitCategoryId",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Habits");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArchived",
                table: "Habits",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Icon",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HabitTrackingEntries",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTrackingEntries", x => x.EntryId);
                    table.ForeignKey(
                        name: "FK_HabitTrackingEntries_Habits_HabitId",
                        column: x => x.HabitId,
                        principalTable: "Habits",
                        principalColumn: "HabitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HabitTrackingEntries_HabitId_EntryDate",
                table: "HabitTrackingEntries",
                columns: new[] { "HabitId", "EntryDate" });
        }
    }
}
