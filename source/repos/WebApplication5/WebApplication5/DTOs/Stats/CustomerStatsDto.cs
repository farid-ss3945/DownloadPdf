namespace WebApplication5.DTOs.Stats
{
    public class CustomerStatsDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public int InvoiceCount { get; set; }
        public decimal TotalSum { get; set; }
    }
}
