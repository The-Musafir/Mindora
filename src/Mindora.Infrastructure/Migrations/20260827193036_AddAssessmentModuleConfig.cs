using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mindora.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssessmentModuleConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentOptions_SelectedOptionId",
                table: "UserAssessmentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentQuestions_QuestionId",
                table: "UserAssessmentAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "UserAssessments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "InProgress");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "UserAssessments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "SeverityLevel",
                table: "AssessmentResults",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Interpretation",
                table: "AssessmentResults",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "AssessmentQuestions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AssessmentQuestionnaires",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "AssessmentOptions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentOptions_SelectedOptionId",
                table: "UserAssessmentAnswers",
                column: "SelectedOptionId",
                principalTable: "AssessmentOptions",
                principalColumn: "OptionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentQuestions_QuestionId",
                table: "UserAssessmentAnswers",
                column: "QuestionId",
                principalTable: "AssessmentQuestions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentOptions_SelectedOptionId",
                table: "UserAssessmentAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentQuestions_QuestionId",
                table: "UserAssessmentAnswers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "UserAssessments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "InProgress",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldDefaultValue: "InProgress");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartedAt",
                table: "UserAssessments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "SeverityLevel",
                table: "AssessmentResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Interpretation",
                table: "AssessmentResults",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QuestionText",
                table: "AssessmentQuestions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "AssessmentQuestionnaires",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OptionText",
                table: "AssessmentOptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentOptions_SelectedOptionId",
                table: "UserAssessmentAnswers",
                column: "SelectedOptionId",
                principalTable: "AssessmentOptions",
                principalColumn: "OptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssessmentAnswers_AssessmentQuestions_QuestionId",
                table: "UserAssessmentAnswers",
                column: "QuestionId",
                principalTable: "AssessmentQuestions",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
