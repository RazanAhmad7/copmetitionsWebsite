using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addRamadanQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRamadanAnswer",
                table: "UserQuizAttempts",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RamadanCompetitionQuestionId",
                table: "UserQuizAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RamadanAnswerText",
                table: "UserAnswers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RamadanCompetitionQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShowFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ShowTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamadanCompetitionQuestions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizAttempts_RamadanCompetitionQuestionId",
                table: "UserQuizAttempts",
                column: "RamadanCompetitionQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizAttempts_RamadanCompetitionQuestions_RamadanCompetitionQuestionId",
                table: "UserQuizAttempts",
                column: "RamadanCompetitionQuestionId",
                principalTable: "RamadanCompetitionQuestions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizAttempts_RamadanCompetitionQuestions_RamadanCompetitionQuestionId",
                table: "UserQuizAttempts");

            migrationBuilder.DropTable(
                name: "RamadanCompetitionQuestions");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizAttempts_RamadanCompetitionQuestionId",
                table: "UserQuizAttempts");

            migrationBuilder.DropColumn(
                name: "IsRamadanAnswer",
                table: "UserQuizAttempts");

            migrationBuilder.DropColumn(
                name: "RamadanCompetitionQuestionId",
                table: "UserQuizAttempts");

            migrationBuilder.DropColumn(
                name: "RamadanAnswerText",
                table: "UserAnswers");
        }
    }
}
