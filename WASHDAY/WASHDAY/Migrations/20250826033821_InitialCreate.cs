using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WASHDAY_202508.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyLedgers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SalesWalkIn = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Sunvida = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Others = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Expenses = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyLedgers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyLedgers");
        }
    }
}
