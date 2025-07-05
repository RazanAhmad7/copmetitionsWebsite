using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addRamadanAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RamadanCompetitionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    Answer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RamadanCompetitionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RamadanCompetitionAnswers_RamadanCompetitionQuestions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "RamadanCompetitionQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RamadanCompetitionAnswers_QuestionId",
                table: "RamadanCompetitionAnswers",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RamadanCompetitionAnswers");
        }
    }
}
