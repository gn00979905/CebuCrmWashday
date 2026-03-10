namespace WASHDAY_202508.DTOs
{
    public class LedgerApiDto
    {
        public int Id { get; set; }
        public string? EntryDate { get; set; } // 日期使用 string 類型
        public decimal SalesWalkIn { get; set; }
        public decimal SanMarino {  get; set; }
        public decimal Sunvida { get; set; }
        public decimal CebuRooms { get; set; }
        public decimal SugbuMercado { get; set; }
        public decimal Others { get; set; }
        public decimal Expenses { get; set; }
        public string? Description { get; set; }
        public decimal TotalSales => SalesWalkIn + SanMarino + Sunvida + CebuRooms + SugbuMercado + Others;
        public decimal NetSales => TotalSales - Expenses;
        // **新增：** 專門給匯出用的費用細項字串
        public string? ExpenseDetails { get; set; }
    }
}
