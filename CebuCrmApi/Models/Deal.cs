using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
    public class Deal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LeadId { get; set; }

        [ForeignKey(nameof(LeadId))]
        public Client? Lead { get; set; }

        [Required]
        public int UnitId { get; set; }

        [ForeignKey(nameof(UnitId))]
        public Unit? Unit { get; set; }

        [Required]
        public string Stage { get; set; } = "Lead";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceSnapshot { get; set; }

        public string? PaymentPlan { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public InvestmentSnapshot? InvestmentSnapshot { get; set; }

        public ICollection<DealPayment> Payments { get; set; } = new List<DealPayment>();

        public ICollection<DealActivity> Activities { get; set; } = new List<DealActivity>();
    }
}
