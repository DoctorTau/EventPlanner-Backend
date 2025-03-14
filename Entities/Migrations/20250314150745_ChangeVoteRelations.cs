using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVoteRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Votes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "VoteId",
                table: "Votes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VotingId",
                table: "Votes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotingId",
                table: "Votes",
                column: "VotingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Votings_VotingId",
                table: "Votes",
                column: "VotingId",
                principalTable: "Votings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Votings_VotingId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Votes_VotingId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "VoteId",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "VotingId",
                table: "Votes");

            migrationBuilder.AlterColumn<int>(
                name: "EventId",
                table: "Votes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Events_EventId",
                table: "Votes",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
