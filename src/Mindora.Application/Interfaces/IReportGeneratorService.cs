using Mindora.Application.DTOs.Reporting;

namespace Mindora.Application.Interfaces
{
    public interface IReportGeneratorService
    {
        /// <summary>
        /// রিপোর্ট অনুযায়ী সব ডেটা Table আকারে ফেরত দেয়।
        /// </summary>
        Task<ReportDataResult> GenerateReportDataAsync(ReportFilterDto filter);
    }

    /// <summary>
    /// Report Data Result — কলাম ও Row এর সমন্বয়।
    /// </summary>
    public class ReportDataResult
    {
        public string ReportTitle { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new();
        public List<List<string>> Rows { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public int TotalRows => Rows.Count;
    }
}