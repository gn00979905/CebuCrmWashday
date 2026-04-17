using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
    public class DealPayment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DealId { get; set; }

        [ForeignKey(nameof(DealId))]
        public Deal? Deal { get; set; }

        [Required]
        public string Type { get; set; } = "Reservation";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }
    }
}
