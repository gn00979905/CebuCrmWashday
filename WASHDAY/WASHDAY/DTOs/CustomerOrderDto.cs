namespace WASHDAY_202508.DTOs
{
    public class CustomerOrderDto
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string? OrderType { get; set; }
        public string? Description { get; set; }
        public int PieceCount { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
        // **修正點：** 拆分成兩個字串欄位
        public string? PickupDate { get; set; }
        public string? PickupTime { get; set; }
    }
}
