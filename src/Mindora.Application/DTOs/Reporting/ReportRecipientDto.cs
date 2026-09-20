namespace Mindora.Application.DTOs.Reporting
{
    public class ReportRecipientDto
    {
        public Guid ReportRecipientId { get; set; }
        public Guid ScheduledReportId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string DeliveryChannel { get; set; } = "Email";
        public DateTime AddedAt { get; set; }
    }
}