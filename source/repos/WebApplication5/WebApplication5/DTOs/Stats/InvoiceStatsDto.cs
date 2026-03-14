namespace WebApplication5.DTOs.Stats
{
    public class InvoiceStatsDto
    {
        public string Service { get; set; } = null!;

        public int InvoiceCount { get; set; }

        public decimal TotalSum { get; set; }
    }
}
