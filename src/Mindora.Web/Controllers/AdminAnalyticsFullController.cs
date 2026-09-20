using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminAnalyticsFullController : Controller
    {
        private readonly IAdminAnalyticsFullService _adminAnalyticsService;
        private readonly IAnalyticsSnapshotService _snapshotService;

        public AdminAnalyticsFullController(
            IAdminAnalyticsFullService adminAnalyticsService,
            IAnalyticsSnapshotService snapshotService)
        {
            _adminAnalyticsService = adminAnalyticsService;
            _snapshotService = snapshotService;
        }

        // GET: /AdminAnalyticsFull
        public async Task<IActionResult> Index()
        {
            var analytics = await _adminAnalyticsService.GetFullAnalyticsAsync();
            return View(analytics);
        }

        // POST: /AdminAnalyticsFull/GenerateSnapshot
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateSnapshot()
        {
            await _snapshotService.GeneratePlatformSnapshotAsync();
            TempData["SnapshotMessage"] = "Snapshot generated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}