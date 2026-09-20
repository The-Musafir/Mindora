using System;

namespace Mindora.Domain.Entities
{
    public class ReportRecipient
    {
        public Guid ReportRecipientId { get; set; }
        public Guid ScheduledReportId { get; set; }
        public Guid UserId { get; set; }
        public string DeliveryChannel { get; set; } = "Email"; // Email, InApp
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ScheduledReport ScheduledReport { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}