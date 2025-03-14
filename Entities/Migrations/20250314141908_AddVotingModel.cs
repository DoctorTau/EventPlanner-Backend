using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddVotingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceVotingId",
                table: "Events",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TimeVotingId",
                table: "Events",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Votings",
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
                    table.PrimaryKey("PK_Votings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votings_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_PlaceVotingId",
                table: "Events",
                column: "PlaceVotingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_TimeVotingId",
                table: "Events",
                column: "TimeVotingId",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Votings_PlaceVotingId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Votings_TimeVotingId",
                table: "Events");

            migrationBuilder.DropTable(
                name: "Votings");

            migrationBuilder.DropIndex(
                name: "IX_Events_PlaceVotingId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_TimeVotingId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "PlaceVotingId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "TimeVotingId",
                table: "Events");
        }
    }
}
