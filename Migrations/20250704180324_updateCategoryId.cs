using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompetitionsWebsite.Migrations
{
    /// <inheritdoc />
    public partial class updateCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizAttempts_Categories_CategoryId",
                table: "UserQuizAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "UserQuizAttempts",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizAttempts_Categories_CategoryId",
                table: "UserQuizAttempts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserQuizAttempts_Categories_CategoryId",
                table: "UserQuizAttempts");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "UserQuizAttempts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserQuizAttempts_Categories_CategoryId",
                table: "UserQuizAttempts",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
