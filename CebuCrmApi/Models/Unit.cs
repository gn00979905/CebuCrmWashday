using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CebuCrmApi.Models
{
    public class Unit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        [Required]
        public string UnitCode { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public decimal SizeSqm { get; set; }

        public int Bedrooms { get; set; } = 1;

        [Required]
        public string Status { get; set; } = "Available";

        public string? PaymentPlan { get; set; }

        public string? Discount { get; set; }

        public string? FloorPlanUrl { get; set; }

        public ICollection<Deal> Deals { get; set; } = new List<Deal>();
    }
}
