using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CebuCrmApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDealCenteredCrm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsForeigner",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOfw",
                table: "Clients",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Preferences",
                table: "Clients",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Developer = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitCode = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    SizeSqm = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    FloorPlanUrl = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Deals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeadId = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Stage = table.Column<string>(type: "TEXT", nullable: false),
                    PriceSnapshot = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    PaymentPlan = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deals_Clients_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deals_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DealActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DealId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealActivities_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DealPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DealId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DealPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DealPayments_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentSnapshots",
                columns: table => new
                {
                    DealId = table.Column<int>(type: "INTEGER", nullable: false),
                    Roi = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    RentalEstimate = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    AirbnbEstimate = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentSnapshots", x => x.DealId);
                    table.ForeignKey(
                        name: "FK_InvestmentSnapshots_Deals_DealId",
                        column: x => x.DealId,
                        principalTable: "Deals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Budget", "CreatedAt", "IsForeigner", "IsOfw", "Nationality", "NextFollowUp", "Notes", "Preferences", "Status" },
                values: new object[] { "4M - 6M PHP", new DateTime(2026, 4, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), false, true, "Filipino", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Looking for an investment-friendly Cebu unit for family use and Airbnb.", "2BR condo near the airport with short-term rental upside", "Contacted" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Budget", "CreatedAt", "IsForeigner", "IsOfw", "Nationality", "NextFollowUp", "Notes", "Preferences", "Status" },
                values: new object[] { "6M - 8M PHP", new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, false, "American", new DateTime(2026, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Needs clear foreign-buyer guidance and ROI estimate.", "RFO condo in IT Park with long-term rental potential", "Site Visit" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Developer", "Location", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "AboitizLand", "Mactan, Cebu", "Mactan Seaview Residences", "Pre-selling" },
                    { 2, "Ayala Land", "IT Park, Cebu City", "IT Park Central", "RFO" }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "FloorPlanUrl", "Price", "ProjectId", "SizeSqm", "Status", "UnitCode" },
                values: new object[,]
                {
                    { 1, "https://example.com/floorplans/sv-1208", 4850000m, 1, 38.5m, "Available", "SV-1208" },
                    { 2, "https://example.com/floorplans/sv-1512", 6350000m, 1, 52.0m, "Reserved", "SV-1512" },
                    { 3, "https://example.com/floorplans/it-0810", 7200000m, 2, 44.0m, "Available", "IT-0810" },
                    { 4, "https://example.com/floorplans/it-1015", 7900000m, 2, 48.0m, "Sold", "IT-1015" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DealActivities_DealId",
                table: "DealActivities",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_DealPayments_DealId",
                table: "DealPayments",
                column: "DealId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_LeadId",
                table: "Deals",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_UnitId",
                table: "Deals",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_ProjectId",
                table: "Units",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DealActivities");

            migrationBuilder.DropTable(
                name: "DealPayments");

            migrationBuilder.DropTable(
                name: "InvestmentSnapshots");

            migrationBuilder.DropTable(
                name: "Deals");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsForeigner",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "IsOfw",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Preferences",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Budget", "NextFollowUp", "Notes", "Status" },
                values: new object[] { null, null, null, "New" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Budget", "NextFollowUp", "Notes", "Status" },
                values: new object[] { null, null, null, "Viewing" });
        }
    }
}
