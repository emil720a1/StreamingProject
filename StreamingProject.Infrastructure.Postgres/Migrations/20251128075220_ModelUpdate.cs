using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreamingProject.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Streams_Id",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_Streams_ChatMessages_ChatId",
                table: "Streams");

            migrationBuilder.DropIndex(
                name: "IX_Streams_ChatId",
                table: "Streams");

            migrationBuilder.RenameColumn(
                name: "SubscriptionTime",
                table: "Subscriptions",
                newName: "SubscriptionAt");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "StreamId",
                table: "ChatMessages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_StreamId",
                table: "ChatMessages",
                column: "StreamId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Streams_StreamId",
                table: "ChatMessages",
                column: "StreamId",
                principalTable: "Streams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Streams_StreamId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_StreamId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StreamId",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "SubscriptionAt",
                table: "Subscriptions",
                newName: "SubscriptionTime");

            migrationBuilder.CreateIndex(
                name: "IX_Streams_ChatId",
                table: "Streams",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Streams_Id",
                table: "ChatMessages",
                column: "Id",
                principalTable: "Streams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Streams_ChatMessages_ChatId",
                table: "Streams",
                column: "ChatId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
