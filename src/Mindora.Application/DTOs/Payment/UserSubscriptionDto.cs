namespace Mindora.Application.DTOs.Payment
{
    public class UserSubscriptionDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid UserId { get; set; }
        public Guid PlanId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}