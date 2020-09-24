using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiTemplate.Values.Infrastructure.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValueItems", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ValueItems",
                columns: new[] { "Id", "Key", "Value" },
                values: new object[] { new Guid("10000000-0000-0000-0000-000000000000"), "asdf", 42 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValueItems");
        }
    }
}
