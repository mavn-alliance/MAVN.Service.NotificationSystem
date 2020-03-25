using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.NotificationSystem.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notification_system");

            migrationBuilder.CreateTable(
                name: "templates",
                schema: "notification_system",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 100, nullable: false),
                    list_of_localization = table.Column<string>(maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_templates", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_templates_name",
                schema: "notification_system",
                table: "templates",
                column: "name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "templates",
                schema: "notification_system");
        }
    }
}
