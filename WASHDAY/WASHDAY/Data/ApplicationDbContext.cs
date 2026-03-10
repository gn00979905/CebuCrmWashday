using Microsoft.EntityFrameworkCore;

namespace WASHDAY_202508.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        // 告訴 DbContext 我們有一個名為 DailyLedgers 的資料表
        // 它對應到 DailyLedger 這個模型
        public DbSet<DailyLedger> DailyLedgers { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<ExpenseItem> ExpenseItems { get; set; } 
        public DbSet<Customer> Customers { get; set; }
    }
}
