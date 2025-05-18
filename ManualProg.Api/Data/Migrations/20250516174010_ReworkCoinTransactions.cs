using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManualProg.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReworkCoinTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostComments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostCommentLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_CoinTransactionId",
                table: "PostLikes",
                column: "CoinTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_CoinTransactionId",
                table: "PostComments",
                column: "CoinTransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentLikes_CoinTransactionId",
                table: "PostCommentLikes",
                column: "CoinTransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PostCommentLikes_CoinTransactions_CoinTransactionId",
                table: "PostCommentLikes",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_CoinTransactions_CoinTransactionId",
                table: "PostComments",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_CoinTransactions_CoinTransactionId",
                table: "PostLikes",
                column: "CoinTransactionId",
                principalTable: "CoinTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostCommentLikes_CoinTransactions_CoinTransactionId",
                table: "PostCommentLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_CoinTransactions_CoinTransactionId",
                table: "PostComments");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_CoinTransactions_CoinTransactionId",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostLikes_CoinTransactionId",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_CoinTransactionId",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentLikes_CoinTransactionId",
                table: "PostCommentLikes");

            migrationBuilder.DropColumn(
                name: "CoinTransactionId",
                table: "PostLikes");

            migrationBuilder.DropColumn(
                name: "CoinTransactionId",
                table: "PostComments");

            migrationBuilder.DropColumn(
                name: "CoinTransactionId",
                table: "PostCommentLikes");
        }
    }
}
