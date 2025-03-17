using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class ChangeVotingToPoll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Votings_PlaceVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Votings_TimeVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Votings_VotingId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "Votings");

            migrationBuilder.CreateTable(
                name: "Polls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Polls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Polls_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Polls_EventId",
                table: "Polls",
                column: "EventId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Polls_VotingId",
                table: "Votes",
                column: "VotingId",
                principalTable: "Polls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_PlaceVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Polls_TimeVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Polls_VotingId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "Polls");

            migrationBuilder.CreateTable(
                name: "Votings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Options = table.Column<List<string>>(type: "text[]", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votings_EventId",
                table: "Votings",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Votings_PlaceVotingId",
                table: "Events",
                column: "PlaceVotingId",
                principalTable: "Votings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Votings_TimeVotingId",
                table: "Events",
                column: "TimeVotingId",
                principalTable: "Votings",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Votings_VotingId",
                table: "Votes",
                column: "VotingId",
                principalTable: "Votings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
