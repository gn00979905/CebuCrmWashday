using System.ComponentModel.DataAnnotations;

namespace CebuCrmApi.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }

        // 興趣與狀態
        public string? Interest { get; set; }
        public string? Status { get; set; } // New, Contacted, Viewing, etc.
        public string? Source { get; set; } // Facebook, Referral, etc.
        public string? Nationality { get; set; }
        public bool IsForeigner { get; set; }
        public bool IsOfw { get; set; }
        public string? Preferences { get; set; }
        public string? InvestmentPurpose { get; set; }
        public string? PreferredArea { get; set; }
        public string? PaymentAbility { get; set; }
        public string? BuyingTimeline { get; set; }

        // 👇 這次新增的房地產進階 CRM 欄位 👇
        public string? Budget { get; set; }         // 預算 (例如: "3M - 5M")
        public DateTime? NextFollowUp { get; set; } // 下次追蹤日 (允許為空 null)
        public string? Notes { get; set; }          // 詳細備註
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}
