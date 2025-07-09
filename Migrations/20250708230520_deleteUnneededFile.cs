using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsWebsite.Migrations
{
    /// <inheritdoc />
    public partial class deleteUnneededFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Participants_ParticipantId",
                table: "UserAnswers");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswers_ParticipantId",
                table: "UserAnswers");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "UserAnswers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "UserAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompetitionId = table.Column<int>(type: "int", nullable: false),
                    HasFinished = table.Column<bool>(type: "bit", nullable: false),
                    HasStarted = table.Column<bool>(type: "bit", nullable: false),
                    IsNotified = table.Column<bool>(type: "bit", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_ParticipantId",
                table: "UserAnswers",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Participants_ParticipantId",
                table: "UserAnswers",
                column: "ParticipantId",
                principalTable: "Participants",
                principalColumn: "Id");
        }
    }
}
