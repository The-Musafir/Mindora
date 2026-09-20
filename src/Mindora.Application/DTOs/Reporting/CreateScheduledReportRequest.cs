namespace Mindora.Application.DTOs.Reporting
{
    public class CreateScheduledReportRequest
    {
        public Guid? TemplateId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Format { get; set; } = "CSV";
        public string Frequency { get; set; } = "Daily"; // Daily, Weekly, Monthly
        public TimeSpan ScheduledTime { get; set; } = new TimeSpan(2, 0, 0);
        public bool IsActive { get; set; } = true;
    }
}