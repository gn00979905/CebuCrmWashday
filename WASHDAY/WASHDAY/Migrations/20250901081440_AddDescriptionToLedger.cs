using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WASHDAY_202508.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToLedger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DailyLedgers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "DailyLedgers");
        }
    }
}
