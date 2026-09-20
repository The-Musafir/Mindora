namespace Mindora.Application.DTOs.Payment
{
    public class InvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public Guid PaymentId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime IssuedAt { get; set; }
    }
}