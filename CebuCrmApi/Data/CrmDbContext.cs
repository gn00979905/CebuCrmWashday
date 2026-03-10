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
        // 在 CrmDbContext.cs 裡面補上這行
        public DbSet<ServiceOrder> ServiceOrders { get; set; }

        // 當資料庫第一次建立時，會自動塞入這些預設資料
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 房產種子資料 (保留之前的)
            modelBuilder.Entity<Property>().HasData(
                new Property { Id = 1, Title = "Mactan Ocean View Condo", Type = "Pre-selling", Price = 4500000, Location = "Mactan, Cebu", Status = "Available", Bedrooms = 2 },
                new Property { Id = 2, Title = "IT Park Studio Unit", Type = "Resale", Price = 3200000, Location = "IT Park, Cebu City", Status = "Negotiating", Bedrooms = 1 },
                new Property { Id = 3, Title = "Busay Mountain Villa", Type = "Resale", Price = 12000000, Location = "Busay", Status = "Sold", Bedrooms = 4 }
            );

            // 客戶種子資料
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Maria Santos", Phone = "09171234567", Email = "maria@example.com", Interest = "Condo", Status = "New", Source = "Facebook" },
                new Client { Id = 2, Name = "John Smith", Phone = "09189876543", Email = "john@example.com", Interest = "House & Lot", Status = "Viewing", Source = "Referral" }
            );
            // 在 OnModelCreating 裡面可以加入一些種子資料 (選填)
            modelBuilder.Entity<ServiceOrder>().HasData(
                new ServiceOrder { Id = 1, ClientName = "Maria Santos", ServiceType = "Laundry", Amount = 500, Status = "Completed", Notes = "Express wash" },
                new ServiceOrder { Id = 2, ServiceType = "Airbnb Cleaning", Amount = 1500, Status = "Pending", Notes = "Unit 1202, IT Park" }
            );
        }
    }
}
