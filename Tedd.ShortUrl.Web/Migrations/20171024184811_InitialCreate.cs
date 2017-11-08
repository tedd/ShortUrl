using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Tedd.ShortUrl.Web.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrl",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorAccessToken = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstVisit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastVisit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MetaData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrl", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrl");
        }
    }
}
