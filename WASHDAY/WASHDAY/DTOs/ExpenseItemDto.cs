namespace WASHDAY_202508.DTOs
{
    public class ExpenseItemDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int DailyLedgerId { get; set; }
    }
}
