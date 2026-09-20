using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ScheduledReport
    {
        public Guid ScheduledReportId { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = "CSV";
        public string Frequency { get; set; } = "Daily"; // Daily, Weekly, Monthly
        public TimeSpan ScheduledTime { get; set; } = new TimeSpan(2, 0, 0); // Runs at 2 AM
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ReportTemplate? Template { get; set; }
        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<ReportRecipient> Recipients { get; set; } = new List<ReportRecipient>();
    }
}