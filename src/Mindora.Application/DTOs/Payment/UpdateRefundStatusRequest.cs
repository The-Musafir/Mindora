namespace Mindora.Application.DTOs.Payment
{
    public class UpdateRefundStatusRequest
    {
        public Guid RefundId { get; set; }
        public string Status { get; set; } = string.Empty; // Approved, Rejected
    }
}