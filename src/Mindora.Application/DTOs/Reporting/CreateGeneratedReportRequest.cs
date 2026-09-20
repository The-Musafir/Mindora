namespace Mindora.Application.DTOs.Reporting
{
    public class CreateGeneratedReportRequest
    {
        public Guid? TemplateId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = "CSV";
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}