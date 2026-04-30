using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CebuCrmApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDemoSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "DealActivities"
                WHERE ("Id" = 1 AND "DealId" = 1 AND "Type" = 'Call' AND "Note" = 'Discussed payment terms and rental strategy.')
                   OR ("Id" = 2 AND "DealId" = 1 AND "Type" = 'Follow-up' AND "Note" = 'Sent reservation confirmation and next payment reminder.')
                   OR ("Id" = 3 AND "DealId" = 2 AND "Type" = 'Site Visit' AND "Note" = 'Walked through amenity deck and model unit.');
                """);

            migrationBuilder.Sql("""
                DELETE FROM "DealPayments"
                WHERE ("Id" = 1 AND "DealId" = 1 AND "Type" = 'Reservation' AND "Note" = 'Reservation fee received')
                   OR ("Id" = 2 AND "DealId" = 1 AND "Type" = 'DP' AND "Note" = 'First down payment installment');
                """);

            migrationBuilder.Sql("""
                DELETE FROM "InvestmentSnapshots"
                WHERE ("DealId" = 1 AND "Roi" = 8.4 AND "RentalEstimate" = 32000 AND "AirbnbEstimate" = 51000)
                   OR ("DealId" = 2 AND "Roi" = 7.1 AND "RentalEstimate" = 35000 AND "AirbnbEstimate" = 56000);
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Deals"
                WHERE ("Id" = 1 AND "LeadId" = 1 AND "UnitId" = 2 AND "Stage" = 'Reservation' AND "Notes" = 'Lead wants a flexible payment plan and Airbnb-ready turnover.')
                   OR ("Id" = 2 AND "LeadId" = 2 AND "UnitId" = 3 AND "Stage" = 'Site Visit' AND "Notes" = 'Schedule a second viewing with spouse on Saturday.');
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Units"
                WHERE "Id" IN (1, 2, 3, 4)
                  AND "UnitCode" IN ('SV-1208', 'SV-1512', 'IT-0810', 'IT-1015')
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "Deals"
                      WHERE "Deals"."UnitId" = "Units"."Id"
                  );
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Projects"
                WHERE "Id" IN (1, 2)
                  AND "Name" IN ('Mactan Seaview Residences', 'IT Park Central')
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "Units"
                      WHERE "Units"."ProjectId" = "Projects"."Id"
                  );
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Clients"
                WHERE "Id" IN (1, 2)
                  AND "Name" IN ('Maria Santos', 'John Smith')
                  AND "Email" IN ('maria@example.com', 'john@example.com')
                  AND NOT EXISTS (
                      SELECT 1
                      FROM "Deals"
                      WHERE "Deals"."LeadId" = "Clients"."Id"
                  );
                """);

            migrationBuilder.Sql("""
                DELETE FROM "Properties"
                WHERE ("Id" = 1 AND "Title" = 'Mactan Ocean View Condo' AND "Location" = 'Mactan, Cebu')
                   OR ("Id" = 2 AND "Title" = 'IT Park Studio Unit' AND "Location" = 'IT Park, Cebu City')
                   OR ("Id" = 3 AND "Title" = 'Busay Mountain Villa' AND "Location" = 'Busay');
                """);

            migrationBuilder.Sql("""
                DELETE FROM "ServiceOrders"
                WHERE ("Id" = 1 AND "ClientName" = 'Maria Santos' AND "ServiceType" = 'Laundry' AND "Notes" = 'Express wash')
                   OR ("Id" = 2 AND "ServiceType" = 'Airbnb Cleaning' AND "Notes" = 'Unit 1202, IT Park');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
