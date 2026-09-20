using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ScheduledReportController : Controller
    {
        private readonly IScheduledReportService _scheduledReportService;
        private readonly IReportTemplateService _templateService;

        public ScheduledReportController(
            IScheduledReportService scheduledReportService,
            IReportTemplateService templateService)
        {
            _scheduledReportService = scheduledReportService;
            _templateService = templateService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /ScheduledReport
        public async Task<IActionResult> Index()
        {
            var reports = await _scheduledReportService.GetAllScheduledReportsAsync();
            return View(reports);
        }

        // GET: /ScheduledReport/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Templates = await _templateService.GetActiveTemplatesAsync();
            return View(new CreateScheduledReportRequest());
        }

        // POST: /ScheduledReport/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateScheduledReportRequest request)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Templates = await _templateService.GetActiveTemplatesAsync();
                return View(request);
            }

            await _scheduledReportService.CreateScheduledReportAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ScheduledReport/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid scheduledReportId, bool isActive)
        {
            await _scheduledReportService.ToggleScheduledReportAsync(scheduledReportId, isActive);
            return RedirectToAction(nameof(Index));
        }

        // POST: /ScheduledReport/RunNow
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunNow(Guid scheduledReportId)
        {
            await _scheduledReportService.RunScheduledReportNowAsync(scheduledReportId);
            TempData["Message"] = "Report triggered successfully.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /ScheduledReport/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid scheduledReportId)
        {
            await _scheduledReportService.DeleteScheduledReportAsync(scheduledReportId);
            return RedirectToAction(nameof(Index));
        }
    }
}