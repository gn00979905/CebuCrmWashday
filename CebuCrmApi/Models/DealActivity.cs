using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
    public class DealActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DealId { get; set; }

        [ForeignKey(nameof(DealId))]
        public Deal? Deal { get; set; }

        [Required]
        public string Type { get; set; } = "Follow-up";

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string? Note { get; set; }
    }
}
