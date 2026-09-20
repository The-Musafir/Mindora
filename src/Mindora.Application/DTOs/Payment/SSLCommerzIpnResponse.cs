namespace Mindora.Application.DTOs.Payment
{
    public class SSLCommerzIpnResponse
    {
        public string Status { get; set; } = string.Empty;
        public string TranId { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string RiskTitle { get; set; } = string.Empty;
    }
}