namespace Mindora.Application.DTOs.Analytics
{
    public class CreateReportRequestDto
    {
        public string ReportType { get; set; } = "UserAnalytics"; // UserAnalytics, AdminAnalytics, Revenue
        public string Format { get; set; } = "CSV"; // CSV, PDF
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}