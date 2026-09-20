namespace Mindora.Application.DTOs.Payment
{
    public class UpdatePayoutStatusRequest
    {
        public Guid PayoutId { get; set; }
        public string Status { get; set; } = string.Empty; // Approved, Rejected, Paid
    }
}