using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_PlaceVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_TimeVotingId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_PlaceVotingId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "TimeVotingId",
                table: "Events",
                newName: "TimePollId");

            migrationBuilder.RenameColumn(
                name: "PlaceVotingId",
                table: "Events",
                newName: "PlacePollId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_TimeVotingId",
                table: "Events",
                newName: "IX_Events_TimePollId");

            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LocationPollId",
                table: "Events",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationPollId",
                table: "Events",
                column: "LocationPollId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Polls_LocationPollId",
                table: "Events",
                column: "LocationPollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Polls_TimePollId",
                table: "Events",
                column: "TimePollId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_LocationPollId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_TimePollId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_LocationPollId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventType",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LocationPollId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "TimePollId",
                table: "Events",
                newName: "TimeVotingId");

            migrationBuilder.RenameColumn(
                name: "PlacePollId",
                table: "Events",
                newName: "PlaceVotingId");

            migrationBuilder.RenameIndex(
                name: "IX_Events_TimePollId",
                table: "Events",
                newName: "IX_Events_TimeVotingId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_PlaceVotingId",
                table: "Events",
                column: "PlaceVotingId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Polls_PlaceVotingId",
                table: "Events",
                column: "PlaceVotingId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Polls_TimeVotingId",
                table: "Events",
                column: "TimeVotingId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
