using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WASHDAY_202508.Data
{
    public class ExpenseItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Category { get; set; } // 例如: "Fare", "Detergent", "Utilities"
        public string? Description { get; set; }
        [Required]
        [Column(TypeName ="decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        public int DailyLedgerId {  get; set; } // Foreign Key
        [ForeignKey(nameof(DailyLedgerId))]
        public virtual DailyLedger DailyLedger { get; set; } // Navigation Property
    }
}
