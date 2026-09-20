using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IGeneratedReportService _generatedReportService;
        private readonly IReportTemplateService _templateService;
        private readonly IReportExportService _exportService;

        public ReportController(
            IGeneratedReportService generatedReportService,
            IReportTemplateService templateService,
            IReportExportService exportService)
        {
            _generatedReportService = generatedReportService;
            _templateService = templateService;
            _exportService = exportService;
        }

        private Guid CurrentUserId => Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : Guid.Empty;

        private bool IsAdmin => User.IsInRole("Admin");

        // ============================
        // USER VIEWS
        // ============================

        // GET: /Report
        public async Task<IActionResult> Index()
        {
            var reports = await _generatedReportService.GetUserReportsAsync(CurrentUserId);
            return View(reports);
        }

        // GET: /Report/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Templates = await _templateService.GetActiveTemplatesAsync();
            return View(new CreateGeneratedReportRequest());
        }

        // POST: /Report/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateGeneratedReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Templates = await _templateService.GetActiveTemplatesAsync();
                return View(request);
            }

            var reportId = await _generatedReportService.RequestReportAsync(CurrentUserId, request);
            TempData["Success"] = "Report requested successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Report/Download/{id}
        public async Task<IActionResult> Download(Guid id)
        {
            try
            {
                var report = await _generatedReportService.GetReportByIdAsync(id);
                if (report == null) return NotFound();

                var bytes = await _generatedReportService.DownloadReportAsync(id, CurrentUserId, IsAdmin);

                var contentType = report.Format switch
                {
                    "PDF" => "application/pdf",
                    "Excel" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "text/csv"
                };

                var extension = report.Format switch
                {
                    "PDF" => "pdf",
                    "Excel" => "xlsx",
                    _ => "csv"
                };

                return File(bytes, contentType, $"Report-{report.ReportType}-{DateTime.UtcNow:yyyyMMdd}.{extension}");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // ============================
        // ADMIN VIEWS
        // ============================

        // GET: /Report/Admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var reports = await _generatedReportService.GetAllReportsAsync();
            return View(reports);
        }

        // POST: /Report/Delete/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
            {
                TempData["Error"] = "Invalid report.";
                return RedirectToAction(nameof(Admin));
            }

            await _generatedReportService.DeleteReportAsync(id);
            TempData["Success"] = "Report deleted.";
            return RedirectToAction(nameof(Admin));
        }

        // ============================
        // QUICK EXPORTS
        // ============================

        // GET: /Report/ExportUserAnalytics
        public async Task<IActionResult> ExportUserAnalytics()
        {
            var filter = new ReportFilterDto
            {
                ReportType = "UserAnalytics",
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow,
                Format = "CSV"
            };

            var bytes = await _exportService.ExportToCsvAsync(filter);
            return File(bytes, "text/csv", $"UserAnalytics-{DateTime.UtcNow:yyyyMMdd}.csv");
        }

        // GET: /Report/ExportRevenue
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportRevenue()
        {
            var filter = new ReportFilterDto
            {
                ReportType = "RevenueReport",
                FromDate = DateTime.UtcNow.AddDays(-30),
                ToDate = DateTime.UtcNow,
                Format = "CSV"
            };

            var bytes = await _exportService.ExportToCsvAsync(filter);
            return File(bytes, "text/csv", $"RevenueReport-{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}