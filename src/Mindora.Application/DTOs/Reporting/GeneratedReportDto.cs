namespace Mindora.Application.DTOs.Reporting
{
    public class GeneratedReportDto
    {
        public Guid GeneratedReportId { get; set; }
        public Guid? TemplateId { get; set; }
        public string? TemplateName { get; set; }
        public Guid RequestedByUserId { get; set; }
        public string RequestedByEmail { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = "CSV";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; } = "Pending";
        public string? FileUrl { get; set; }
        public long? FileSizeBytes { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}