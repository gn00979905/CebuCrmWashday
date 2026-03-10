namespace CebuCrmApi.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Pre-selling 或 Resale
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
    }
}
