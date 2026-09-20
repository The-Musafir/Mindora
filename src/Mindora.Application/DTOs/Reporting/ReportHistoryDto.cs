namespace Mindora.Application.DTOs.Reporting
{
    public class ReportHistoryDto
    {
        public Guid ReportId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RequestedByEmail { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}