using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Quickpaste.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pastes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    BlobUrl = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    IsPublic = table.Column<bool>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    QuickLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pastes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadLinks",
                columns: table => new
                {
                    QuickLink = table.Column<string>(nullable: false),
                    AllowFile = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadLinks", x => x.QuickLink);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pastes_IsPublic",
                table: "Pastes",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_Pastes_QuickLink",
                table: "Pastes",
                column: "QuickLink",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pastes");

            migrationBuilder.DropTable(
                name: "UploadLinks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
