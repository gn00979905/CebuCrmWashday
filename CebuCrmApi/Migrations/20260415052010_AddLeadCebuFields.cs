using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuCrmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadCebuFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuyingTimeline",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestmentPurpose",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentAbility",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredArea",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BuyingTimeline", "InvestmentPurpose", "PaymentAbility", "PreferredArea" },
                values: new object[] { "Within 3 Months", "Rental Income", "Bank Financing", "Mactan / Airport Area" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "BuyingTimeline", "InvestmentPurpose", "PaymentAbility", "PreferredArea" },
                values: new object[] { "Immediate", "Rental", "Cash", "IT Park / Cebu City" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyingTimeline",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "InvestmentPurpose",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PaymentAbility",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "PreferredArea",
                table: "Clients");
        }
    }
}
