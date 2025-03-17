using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFieldsNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Polls_VotingId",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "VotingId",
                table: "Votes",
                newName: "PollId");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_VotingId",
                table: "Votes",
                newName: "IX_Votes_PollId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Polls_PollId",
                table: "Votes",
                column: "PollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Polls_PollId",
                table: "Votes");

            migrationBuilder.RenameColumn(
                name: "PollId",
                table: "Votes",
                newName: "VotingId");

            migrationBuilder.RenameIndex(
                name: "IX_Votes_PollId",
                table: "Votes",
                newName: "IX_Votes_VotingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Polls_VotingId",
                table: "Votes",
                column: "VotingId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
