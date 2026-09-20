namespace Mindora.Application.DTOs.Reporting
{
    public class ScheduledReportDto
    {
        public Guid ScheduledReportId { get; set; }
        public Guid? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByEmail { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = "CSV";
        public string Frequency { get; set; } = "Daily";
        public TimeSpan ScheduledTime { get; set; }
        public DateTime? LastRunAt { get; set; }
        public DateTime? NextRunAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RecipientCount { get; set; }
    }
}