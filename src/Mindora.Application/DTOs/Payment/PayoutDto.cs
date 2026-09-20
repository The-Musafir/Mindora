namespace Mindora.Application.DTOs.Payment
{
    public class PayoutDto
    {
        public Guid PayoutId { get; set; }
        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string AccountDetails { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
    }
}