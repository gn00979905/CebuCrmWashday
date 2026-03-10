using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WASHDAY_202508.Data
{
    public class CustomerOrder
    {
        [Key]
        public int Id { get; set; } // 對應 serial#

        [Required]
        public DateTime OrderDate { get; set; } // 對應 Date

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } // 對應 Name

        [MaxLength(50)]
        public string? OrderType { get; set; } // 對應 Type

        public string? Description { get; set; } // 對應 Description

        public int PieceCount { get; set; } // 對應 pieces

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } // 對應 Amount

        public bool IsPaid { get; set; } // 對應 IsPaid，使用布林值更佳
        // 將 PickupDate 重新命名為 PickupDateTime
        public DateTime? PickupDateTime { get; set; } // 對應 Out Date，可為 null
    }
}
