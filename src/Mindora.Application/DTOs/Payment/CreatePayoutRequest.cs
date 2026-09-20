namespace Mindora.Application.DTOs.Payment
{
    public class CreatePayoutRequest
    {
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Bank";
        public string AccountDetails { get; set; } = string.Empty;
    }
}