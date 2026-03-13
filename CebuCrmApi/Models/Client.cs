namespace CebuCrmApi.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Interest { get; set; } = string.Empty; // 客戶感興趣的物件類型
        public string Status { get; set; } = "New";          // New, Viewing, Closed
        public string Source { get; set; } = "Unknown";     // FB, Referral, Airbnb, etc.
        public DateTime CreatedAt { get; set; }
    }
}
