using System;

namespace Mindora.Domain.Entities
{
    public class ReportRequest
    {
        public Guid ReportId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public Guid? TemplateId { get; set; }
        public string ReportType { get; set; } = "UserAnalytics";
        public string Format { get; set; } = "CSV";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? FileUrl { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public virtual User RequestedByUser { get; set; } = null!;
        public virtual ReportTemplate? Template { get; set; }
    }
}