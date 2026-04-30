using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuCrmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectCategoryAndUnitBedrooms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bedrooms",
                table: "Units",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Projects",
                type: "TEXT",
                nullable: false,
                defaultValue: "House and Lot");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bedrooms",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Projects");
        }
    }
}
