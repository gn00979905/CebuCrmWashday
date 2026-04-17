using System.ComponentModel.DataAnnotations;

namespace CebuCrmApi.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Developer { get; set; }

        [Required]
        public string Type { get; set; } = "Pre-selling";

        public string? Location { get; set; }

        public ICollection<Unit> Units { get; set; } = new List<Unit>();
    }
}
