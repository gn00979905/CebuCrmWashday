using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
    public class InvestmentSnapshot
    {
        [Key]
        [ForeignKey(nameof(Deal))]
        public int DealId { get; set; }

        public Deal? Deal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Roi { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal RentalEstimate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AirbnbEstimate { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
