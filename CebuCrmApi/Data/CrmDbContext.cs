using Microsoft.EntityFrameworkCore;
using CebuCrmApi.Models;
namespace CebuCrmApi.Data
{
    public class CrmDbContext:DbContext
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

        // 代表資料庫裡的一張資料表 (Table)
        public DbSet<Property> Properties { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Deal> Deals { get; set; }
        public DbSet<DealPayment> DealPayments { get; set; }
        public DbSet<DealActivity> DealActivities { get; set; }
        public DbSet<InvestmentSnapshot> InvestmentSnapshots { get; set; }

        // 當資料庫第一次建立時，會自動塞入這些預設資料
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Deal>()
                .HasOne(d => d.Lead)
                .WithMany(c => c.Deals)
                .HasForeignKey(d => d.LeadId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Deal>()
                .HasOne(d => d.Unit)
                .WithMany(u => u.Deals)
                .HasForeignKey(d => d.UnitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InvestmentSnapshot>()
                .HasOne(i => i.Deal)
                .WithOne(d => d.InvestmentSnapshot)
                .HasForeignKey<InvestmentSnapshot>(i => i.DealId)
                .OnDelete(DeleteBehavior.Cascade);

            // 房產種子資料 (保留之前的)
            modelBuilder.Entity<Property>().HasData(
                new Property { Id = 1, Title = "Mactan Ocean View Condo", Type = "Pre-selling", Category = "Beach Front", Price = 4500000, Location = "Mactan, Cebu", Status = "Available", Bedrooms = 2 },
                new Property { Id = 2, Title = "IT Park Studio Unit", Type = "Resale", Category = "House and Lot", Price = 3200000, Location = "IT Park, Cebu City", Status = "Negotiating", Bedrooms = 1 },
                new Property { Id = 3, Title = "Busay Mountain Villa", Type = "Resale", Category = "Resort", Price = 12000000, Location = "Busay", Status = "Sold", Bedrooms = 4 }
            );

            // 客戶種子資料
            modelBuilder.Entity<Client>().HasData(
                new Client
                {
                    Id = 1,
                    Name = "Maria Santos",
                    Phone = "09171234567",
                    Email = "maria@example.com",
                    Interest = "Condo",
                    Preferences = "2BR condo near the airport with short-term rental upside",
                    Status = "Contacted",
                    Source = "Facebook",
                    Budget = "4M - 6M PHP",
                    Nationality = "Filipino",
                    IsForeigner = false,
                    IsOfw = true,
                    InvestmentPurpose = "Rental Income",
                    PreferredArea = "Mactan / Airport Area",
                    PaymentAbility = "Bank Financing",
                    BuyingTimeline = "Within 3 Months",
                    Notes = "Looking for an investment-friendly Cebu unit for family use and Airbnb.",
                    NextFollowUp = new DateTime(2026, 4, 18),
                    CreatedAt = new DateTime(2026, 4, 2)
                },
                new Client
                {
                    Id = 2,
                    Name = "John Smith",
                    Phone = "09189876543",
                    Email = "john@example.com",
                    Interest = "House & Lot",
                    Preferences = "RFO condo in IT Park with long-term rental potential",
                    Status = "Site Visit",
                    Source = "Referral",
                    Budget = "6M - 8M PHP",
                    Nationality = "American",
                    IsForeigner = true,
                    IsOfw = false,
                    InvestmentPurpose = "Rental",
                    PreferredArea = "IT Park / Cebu City",
                    PaymentAbility = "Cash",
                    BuyingTimeline = "Immediate",
                    Notes = "Needs clear foreign-buyer guidance and ROI estimate.",
                    NextFollowUp = new DateTime(2026, 4, 16),
                    CreatedAt = new DateTime(2026, 4, 1)
                }
            );

            modelBuilder.Entity<ServiceOrder>().HasData(
                new ServiceOrder { Id = 1, ClientName = "Maria Santos", ServiceType = "Laundry", Amount = 500, Status = "Completed", Notes = "Express wash" },
                new ServiceOrder { Id = 2, ServiceType = "Airbnb Cleaning", Amount = 1500, Status = "Pending", Notes = "Unit 1202, IT Park" }
            );

            modelBuilder.Entity<Project>().HasData(
                new Project { Id = 1, Name = "Mactan Seaview Residences", Developer = "AboitizLand", Type = "Pre-selling", Location = "Mactan, Cebu" },
                new Project { Id = 2, Name = "IT Park Central", Developer = "Ayala Land", Type = "RFO", Location = "IT Park, Cebu City" }
            );

            modelBuilder.Entity<Unit>().HasData(
                new Unit { Id = 1, ProjectId = 1, UnitCode = "SV-1208", Price = 4850000, SizeSqm = 38.5m, Status = "Available", FloorPlanUrl = "https://example.com/floorplans/sv-1208" },
                new Unit { Id = 2, ProjectId = 1, UnitCode = "SV-1512", Price = 6350000, SizeSqm = 52.0m, Status = "Reserved", FloorPlanUrl = "https://example.com/floorplans/sv-1512" },
                new Unit { Id = 3, ProjectId = 2, UnitCode = "IT-0810", Price = 7200000, SizeSqm = 44.0m, Status = "Available", FloorPlanUrl = "https://example.com/floorplans/it-0810" },
                new Unit { Id = 4, ProjectId = 2, UnitCode = "IT-1015", Price = 7900000, SizeSqm = 48.0m, Status = "Sold", FloorPlanUrl = "https://example.com/floorplans/it-1015" }
            );

            modelBuilder.Entity<Deal>().HasData(
                new Deal
                {
                    Id = 1,
                    LeadId = 1,
                    UnitId = 2,
                    Stage = "Reservation",
                    PriceSnapshot = 6350000,
                    PaymentPlan = "30% DP over 24 months, 70% bank financing",
                    Notes = "Lead wants a flexible payment plan and Airbnb-ready turnover.",
                    CreatedAt = new DateTime(2026, 4, 6)
                },
                new Deal
                {
                    Id = 2,
                    LeadId = 2,
                    UnitId = 3,
                    Stage = "Site Visit",
                    PriceSnapshot = 7200000,
                    PaymentPlan = "20% DP then cash-out on turnover",
                    Notes = "Schedule a second viewing with spouse on Saturday.",
                    CreatedAt = new DateTime(2026, 4, 5)
                }
            );

            modelBuilder.Entity<DealPayment>().HasData(
                new DealPayment { Id = 1, DealId = 1, Type = "Reservation", Amount = 50000, Date = new DateTime(2026, 4, 7), Note = "Reservation fee received" },
                new DealPayment { Id = 2, DealId = 1, Type = "DP", Amount = 180000, Date = new DateTime(2026, 4, 12), Note = "First down payment installment" }
            );

            modelBuilder.Entity<DealActivity>().HasData(
                new DealActivity { Id = 1, DealId = 1, Type = "Call", Date = new DateTime(2026, 4, 6), Note = "Discussed payment terms and rental strategy." },
                new DealActivity { Id = 2, DealId = 1, Type = "Follow-up", Date = new DateTime(2026, 4, 12), Note = "Sent reservation confirmation and next payment reminder." },
                new DealActivity { Id = 3, DealId = 2, Type = "Site Visit", Date = new DateTime(2026, 4, 9), Note = "Walked through amenity deck and model unit." }
            );

            modelBuilder.Entity<InvestmentSnapshot>().HasData(
                new InvestmentSnapshot { DealId = 1, Roi = 8.4m, RentalEstimate = 32000, AirbnbEstimate = 51000, UpdatedAt = new DateTime(2026, 4, 7) },
                new InvestmentSnapshot { DealId = 2, Roi = 7.1m, RentalEstimate = 35000, AirbnbEstimate = 56000, UpdatedAt = new DateTime(2026, 4, 9) }
            );
        }
    }
}
