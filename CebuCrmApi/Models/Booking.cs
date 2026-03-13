using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
   // 2. 訂單實體 (Booking Entity)
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room? Room { get; set; }

        // 這裡連結你現有的 CRM 客戶實體
        [Required]
        public int CustomerId { get; set; }

        // 假設你的 CRM 客戶實體叫 Customer
        // public Customer? Customer { get; set; }

        [Required]
        public string GuestName { get; set; } = string.Empty; // 冗餘儲存以便快速顯示

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        public int GuestCount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;

        //public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; }
    }
    // 訂單狀態列舉
    public enum BookingStatus
    {
        Confirmed,   // 已確認（訂金已付）
        CheckedIn,   // 已入住
        Completed,   // 已退房/完成
        Cancelled,   // 已取消
        Maintenance  // 維修占用
    }
}
