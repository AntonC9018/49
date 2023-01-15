using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourtynine.DataAccess.Migrations
{
    public partial class AllowDifferentAuthenticationMethods : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllowedAuthenticationScheme",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllowedAuthenticationScheme", x => new { x.UserId, x.SchemeName });
                    table.ForeignKey(
                        name: "FK_AllowedAuthenticationScheme_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllowedAuthenticationScheme");
        }
    }
}
