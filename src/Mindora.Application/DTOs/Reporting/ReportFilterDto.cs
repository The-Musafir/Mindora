namespace Mindora.Application.DTOs.Reporting
{
    public class ReportFilterDto
    {
        public string ReportType { get; set; } = string.Empty;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Category { get; set; }
        public Guid? UserId { get; set; }
        public string Format { get; set; } = "CSV";
    }
}