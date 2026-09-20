namespace Mindora.Application.DTOs.Reporting
{
    public class AddReportRecipientRequest
    {
        public Guid ScheduledReportId { get; set; }
        public Guid UserId { get; set; }
        public string DeliveryChannel { get; set; } = "Email";
    }
}