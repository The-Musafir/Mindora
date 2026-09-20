namespace Mindora.Application.DTOs.Payment
{
    public class CreateRefundRequestRequest
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}