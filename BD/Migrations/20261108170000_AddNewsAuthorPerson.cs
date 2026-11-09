using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BD.Migrations
{
    public partial class AddNewsAuthorPerson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AuthorPersonId",
                table: "News",
                type: "bigint",
                nullable: true);

            migrationBuilder.Sql("UPDATE `News` AS n INNER JOIN `People` AS p ON p.`UserId` = CAST(n.`CreatedBy` AS CHAR(36)) SET n.`AuthorPersonId` = p.`Id`;");

            migrationBuilder.CreateIndex(
                name: "IX_News_AuthorPersonId",
                table: "News",
                column: "AuthorPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_People_AuthorPersonId",
                table: "News",
                column: "AuthorPersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_People_AuthorPersonId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_AuthorPersonId",
                table: "News");

            migrationBuilder.DropColumn(
                name: "AuthorPersonId",
                table: "News");
        }
    }
}
