using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManualProg.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReworkComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_PostComments_ReplyToCommentId",
                table: "PostComments");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "PostComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_PostComments_ReplyToCommentId",
                table: "PostComments",
                column: "ReplyToCommentId",
                principalTable: "PostComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostComments_PostComments_ReplyToCommentId",
                table: "PostComments");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "PostComments");

            migrationBuilder.AddForeignKey(
                name: "FK_PostComments_PostComments_ReplyToCommentId",
                table: "PostComments",
                column: "ReplyToCommentId",
                principalTable: "PostComments",
                principalColumn: "Id");
        }
    }
}
