namespace CebuCrmApi.Models
{
    public class ServiceOrder
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty; // Airbnb, Laundry, Car Rental, Cleaning
        public decimal Amount { get; set; }
        public string Status { get; set; } = "Pending";        // Pending, In Progress, Completed
        //public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime OrderDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
