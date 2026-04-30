using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuCrmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: "House and Lot");

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "Category",
                value: "Beach Front");

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                column: "Category",
                value: "House and Lot");

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "Category",
                value: "Resort");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Properties");
        }
    }
}
