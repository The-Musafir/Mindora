namespace Mindora.Application.DTOs.Analytics
{
    public class ReportRequestDto
    {
        public Guid ReportId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public string RequestedByEmail { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}