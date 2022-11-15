using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fourtynine.DataAccess.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Postings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    Pricing_BargainKinds = table.Column<byte>(type: "tinyint", nullable: true),
                    Pricing_Price = table.Column<decimal>(type: "money", nullable: true),
                    Pricing_PriceMax = table.Column<decimal>(type: "money", nullable: true),
                    Location_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location_Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Location_Latitude = table.Column<double>(type: "float", nullable: true),
                    Location_Longitude = table.Column<double>(type: "float", nullable: true),
                    RealEstate_Kind = table.Column<string>(type: "varchar(16)", nullable: true),
                    RealEstate_SpacePurpose = table.Column<string>(type: "varchar(16)", nullable: true),
                    RealEstate_Area = table.Column<decimal>(type: "decimal(8,1)", nullable: true),
                    RealEstate_Rooms = table.Column<int>(type: "int", nullable: true),
                    Vehicle_Year = table.Column<int>(type: "int", nullable: true),
                    Vehicle_Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Vehicle_Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Postings_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Picture",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostingId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Picture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Picture_Postings_PostingId",
                        column: x => x.PostingId,
                        principalTable: "Postings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Picture_PostingId",
                table: "Picture",
                column: "PostingId");

            migrationBuilder.CreateIndex(
                name: "IX_Postings_AuthorId",
                table: "Postings",
                column: "AuthorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Picture");

            migrationBuilder.DropTable(
                name: "Postings");

            migrationBuilder.DropTable(
                name: "Authors");
        }
    }
}
