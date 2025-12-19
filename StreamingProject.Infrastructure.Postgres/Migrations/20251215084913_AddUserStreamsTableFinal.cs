using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddUserStreamsTableFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StreamUsers");

            migrationBuilder.CreateTable(
                name: "UserStreams",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStreams", x => new { x.StreamId, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserStreams");

            migrationBuilder.CreateTable(
                name: "StreamUsers",
                columns: table => new
                {
                    StreamId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StreamUsers", x => new { x.StreamId, x.UserId });
                });
        }
    }
}
