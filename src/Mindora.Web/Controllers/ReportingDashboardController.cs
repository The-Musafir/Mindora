using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportingDashboardController : Controller
    {
        private readonly IGeneratedReportService _generatedReportService;
        private readonly IScheduledReportService _scheduledReportService;
        private readonly IReportTemplateService _templateService;

        public ReportingDashboardController(
            IGeneratedReportService generatedReportService,
            IScheduledReportService scheduledReportService,
            IReportTemplateService templateService)
        {
            _generatedReportService = generatedReportService;
            _scheduledReportService = scheduledReportService;
            _templateService = templateService;
        }

        // GET: /ReportingDashboard
        public async Task<IActionResult> Index()
        {
            var recentReports = await _generatedReportService.GetAllReportsAsync();
            var activeSchedules = await _scheduledReportService.GetActiveScheduledReportsAsync();
            var templates = await _templateService.GetActiveTemplatesAsync();

            ViewBag.RecentReports = recentReports.Take(10).ToList();
            ViewBag.ActiveSchedules = activeSchedules.Take(10).ToList();
            ViewBag.TemplatesCount = templates.Count;

            return View();
        }
    }
}