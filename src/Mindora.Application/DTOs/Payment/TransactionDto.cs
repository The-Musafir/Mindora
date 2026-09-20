namespace Mindora.Application.DTOs.Payment
{
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}