using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace CebuCrmApi.Models
{
    // 房間類型列舉
    public enum RoomType
    {
        Studio,
        OneBedroom,
        TwoBedroom,
        FamilySuite,
        Penthouse
    }

    // 房間狀態列舉
    public enum RoomStatus
    {
        Clean,       // 乾淨（可入住）
        Dirty,       // 髒亂（待打掃）
        Occupied,    // 入住中
        Maintenance  // 維修中
    }
    // 1. 房間實體 (Room Entity)
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoomNumber { get; set; } = string.Empty; // 房號，例如 "101"

        [Required]
        public RoomType Type { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.Clean;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerNight { get; set; }

        public string? Description { get; set; }

        // 導覽屬性：一個房間可以有多筆訂單
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }


 
}
