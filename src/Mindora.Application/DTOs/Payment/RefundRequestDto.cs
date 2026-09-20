namespace Mindora.Application.DTOs.Payment
{
    public class RefundRequestDto
    {
        public Guid RefundId { get; set; }
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}