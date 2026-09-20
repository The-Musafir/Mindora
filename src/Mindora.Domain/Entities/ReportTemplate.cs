using System;
using System.Collections.Generic;

namespace Mindora.Domain.Entities
{
    public class ReportTemplate
    {
        public Guid TemplateId { get; set; }
        public string TemplateKey { get; set; } = string.Empty; // e.g., RevenueReport, UserGrowth
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "Business"; // Business, User, Wellness, Community, Compliance
        public string DefaultFormat { get; set; } = "CSV"; // CSV, Excel, PDF
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();
        public virtual ICollection<ScheduledReport> ScheduledReports { get; set; } = new List<ScheduledReport>();
    }
}