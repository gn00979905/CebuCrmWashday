using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WASHDAY_202508.Migrations
{
    /// <inheritdoc />
    public partial class AddSanMarino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SanMarino",
                table: "DailyLedgers",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SanMarino",
                table: "DailyLedgers");
        }
    }
}
