namespace Mindora.Application.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; } // ✅
        public DateTime CreatedAt { get; set; }
    }
}