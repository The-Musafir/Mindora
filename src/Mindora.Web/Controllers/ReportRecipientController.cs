using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Reporting;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportRecipientController : Controller
    {
        private readonly IReportRecipientService _recipientService;
        private readonly IScheduledReportService _scheduledReportService;

        public ReportRecipientController(
            IReportRecipientService recipientService,
            IScheduledReportService scheduledReportService)
        {
            _recipientService = recipientService;
            _scheduledReportService = scheduledReportService;
        }

        // GET: /ReportRecipient?ScheduledReportId=xxx
        public async Task<IActionResult> Index(Guid scheduledReportId)
        {
            var report = await _scheduledReportService.GetScheduledReportByIdAsync(scheduledReportId);
            if (report == null) return NotFound();

            ViewBag.ScheduledReportId = scheduledReportId;
            ViewBag.ReportType = report.ReportType;
            var recipients = await _recipientService.GetRecipientsAsync(scheduledReportId);
            return View(recipients);
        }

        // POST: /ReportRecipient/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddReportRecipientRequest request)
        {
            try
            {
                await _recipientService.AddRecipientAsync(request);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index), new { scheduledReportId = request.ScheduledReportId });
        }

        // POST: /ReportRecipient/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(Guid reportRecipientId, Guid scheduledReportId)
        {
            await _recipientService.RemoveRecipientAsync(reportRecipientId);
            return RedirectToAction(nameof(Index), new { scheduledReportId });
        }

        // POST: /ReportRecipient/RemoveAll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAll(Guid scheduledReportId)
        {
            await _recipientService.RemoveAllRecipientsAsync(scheduledReportId);
            return RedirectToAction(nameof(Index), new { scheduledReportId });
        }
    }
}