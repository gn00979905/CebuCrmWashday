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
        }
    }
}
