using System;

namespace Mindora.Domain.Entities
{
    public class GeneratedReport
    {
        public Guid GeneratedReportId { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public string ReportType { get; set; } = string.Empty; // mirrors ReportTemplate.TemplateKey
        public string Format { get; set; } = "CSV";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Completed, Failed
        public string? FileUrl { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public virtual ReportTemplate? Template { get; set; }
        public virtual User RequestedByUser { get; set; } = null!;
    }
}