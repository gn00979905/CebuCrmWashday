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

            migrationBuilder.Sql("""
                UPDATE "Properties"
                SET "Category" = 'Beach Front'
                WHERE "Id" = 1 AND "Title" = 'Mactan Ocean View Condo';
                """);

            migrationBuilder.Sql("""
                UPDATE "Properties"
                SET "Category" = 'House and Lot'
                WHERE "Id" = 2 AND "Title" = 'IT Park Studio Unit';
                """);

            migrationBuilder.Sql("""
                UPDATE "Properties"
                SET "Category" = 'Resort'
                WHERE "Id" = 3 AND "Title" = 'Busay Mountain Villa';
                """);
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
