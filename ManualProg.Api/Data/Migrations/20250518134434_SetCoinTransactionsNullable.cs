using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManualProg.Api.Migrations
{
    /// <inheritdoc />
    public partial class SetCoinTransactionsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostLikes_CoinTransactionId",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_CoinTransactionId",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentLikes_CoinTransactionId",
                table: "PostCommentLikes");

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostLikes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostComments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostCommentLikes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_PostLikes_CoinTransactionId",
                table: "PostLikes",
                column: "CoinTransactionId",
                unique: true,
                filter: "[CoinTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PostComments_CoinTransactionId",
                table: "PostComments",
                column: "CoinTransactionId",
                unique: true,
                filter: "[CoinTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PostCommentLikes_CoinTransactionId",
                table: "PostCommentLikes",
                column: "CoinTransactionId",
                unique: true,
                filter: "[CoinTransactionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostLikes_CoinTransactionId",
                table: "PostLikes");

            migrationBuilder.DropIndex(
                name: "IX_PostComments_CoinTransactionId",
                table: "PostComments");

            migrationBuilder.DropIndex(
                name: "IX_PostCommentLikes_CoinTransactionId",
                table: "PostCommentLikes");

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostComments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CoinTransactionId",
                table: "PostCommentLikes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
        }
    }
}
