using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WASHDAY_202508.Data
{
    public class DailyLedger
    {
        [Key] // 代表這是主鍵
        public int Id { get; set; }

        [Required] // 代表這是必填欄位
        [DataType(DataType.Date)] // 指定資料類型為日期
        [Display(Name = "Date")] // 在頁面上顯示的名稱
        public DateTime EntryDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")] // 指定在資料庫中的精確類型
        [Display(Name = "Sales/Walk-in")]
        public decimal SalesWalkIn { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "San Marino")]
        public decimal SanMarino { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Sunvida { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cebu Rooms")]
        public decimal CebuRooms { get; set; } // <--- 新增這個屬性
        [Column(TypeName = "decimal(18, 2)")]
        public decimal SugbuMercado { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Others { get; set; }

        //[Column(TypeName = "decimal(18, 2)")]
        //public decimal Expenses { get; set; }
        // --- 建立與 ExpenseItem 的關聯 ---
        public virtual ICollection<ExpenseItem> ExpenseItems { get; set; } = new List<ExpenseItem>();

        // --- 將 Expenses 改為不儲存到資料庫的計算屬性 ---
        [NotMapped] // 告訴 EF Core 不要為此屬性在資料庫中建立欄位
        [Display(Name = "Expenses")]
        public decimal Expenses => ExpenseItems?.Sum(e => e.Amount) ?? 0;

        [Display(Name = "Description / Notes")]
        public string? Description { get; set; } // 新增的備註欄位

        // 唯讀屬性，會自動計算，不會儲存在資料庫
        [NotMapped]
        [Display(Name = "Total Sales")]
        public decimal TotalSales => SalesWalkIn + SanMarino+ Sunvida + CebuRooms + SugbuMercado + Others;

        [NotMapped]
        [Display(Name = "Net Sales")]
        public decimal NetSales => TotalSales - Expenses;
    }
}
