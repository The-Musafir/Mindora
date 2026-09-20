namespace Mindora.Application.DTOs.Payment
{
    public class CreateSubscriptionRequest
    {
        public Guid PlanId { get; set; }
        // UserId Controller-এ CurrentUserId থেকে নেওয়া হবে
    }
}