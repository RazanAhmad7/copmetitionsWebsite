using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsWebsite.Migrations
{
    /// <inheritdoc />
    public partial class addSpecialQuizzes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialQuizAssignmentId",
                table: "UserQuizAttempts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpecialQuizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialQuizzes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpecialQuizAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialQuizId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialQuizAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialQuizAssignments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialQuizAssignments_SpecialQuizzes_SpecialQuizId",
                        column: x => x.SpecialQuizId,
                        principalTable: "SpecialQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialQuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialQuizId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialQuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialQuizQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialQuizQuestions_SpecialQuizzes_SpecialQuizId",
                        column: x => x.SpecialQuizId,
                        principalTable: "SpecialQuizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserQuizAttempts_SpecialQuizAssignmentId",
                table: "UserQuizAttempts",
                column: "SpecialQuizAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialQuizAssignments_SpecialQuizId",
                table: "SpecialQuizAssignments",
                column: "SpecialQuizId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialQuizAssignments_UserId",
                table: "SpecialQuizAssignments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialQuizQuestions_QuestionId",
                table: "SpecialQuizQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialQuizQuestions_SpecialQuizId",
                table: "SpecialQuizQuestions",
                column: "SpecialQuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizAttempts_SpecialQuizAssignments_SpecialQuizAssignmentId",
                table: "UserQuizAttempts",
                column: "SpecialQuizAssignmentId",
                principalTable: "SpecialQuizAssignments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizAttempts_SpecialQuizAssignments_SpecialQuizAssignmentId",
                table: "UserQuizAttempts");

            migrationBuilder.DropTable(
                name: "SpecialQuizAssignments");

            migrationBuilder.DropTable(
                name: "SpecialQuizQuestions");

            migrationBuilder.DropTable(
                name: "SpecialQuizzes");

            migrationBuilder.DropIndex(
                name: "IX_UserQuizAttempts_SpecialQuizAssignmentId",
                table: "UserQuizAttempts");

            migrationBuilder.DropColumn(
                name: "SpecialQuizAssignmentId",
                table: "UserQuizAttempts");
        }
    }
}
