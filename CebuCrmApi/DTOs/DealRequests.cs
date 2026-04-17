namespace CebuCrmApi.DTOs
{
    public class CreateDealRequest
    {
        public int LeadId { get; set; }
        public int UnitId { get; set; }
        public string? Stage { get; set; }
        public decimal? PriceSnapshot { get; set; }
        public string? PaymentPlan { get; set; }
        public string? Notes { get; set; }
        public decimal? Roi { get; set; }
        public decimal? RentalEstimate { get; set; }
        public decimal? AirbnbEstimate { get; set; }
    }

    public class UpdateDealRequest
    {
        public string? Stage { get; set; }
        public decimal? PriceSnapshot { get; set; }
        public string? PaymentPlan { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateDealPaymentRequest
    {
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime? Date { get; set; }
        public string? Note { get; set; }
    }

    public class CreateDealActivityRequest
    {
        public string? Type { get; set; }
        public DateTime? Date { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateInvestmentSnapshotRequest
    {
        public decimal Roi { get; set; }
        public decimal RentalEstimate { get; set; }
        public decimal AirbnbEstimate { get; set; }
    }
}
