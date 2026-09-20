namespace Mindora.Application.DTOs.Analytics
{
    public class RevenueTrendDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int PaymentCount { get; set; }
    }
}