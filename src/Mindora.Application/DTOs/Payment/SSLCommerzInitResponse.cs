namespace Mindora.Application.DTOs.Payment
{
    public class SSLCommerzInitResponse
    {
        public string Status { get; set; } = string.Empty;
        public string FailedReason { get; set; } = string.Empty;
        public string SessionKey { get; set; } = string.Empty;
        public string GatewayPageURL { get; set; } = string.Empty;
    }
}