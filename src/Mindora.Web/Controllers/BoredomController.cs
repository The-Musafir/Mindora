using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class BoredomController : Controller
    {
        private readonly IBoredomService _boredomService;

        public BoredomController(IBoredomService boredomService)
        {
            _boredomService = boredomService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /Boredom
        public async Task<IActionResult> Index()
        {
            var stats = await _boredomService.GetStatsAsync(CurrentUserId);
            var categories = await _boredomService.GetCategoriesAsync();

            ViewBag.Stats = stats;
            ViewBag.Categories = categories;

            return View();
        }

        // GET: /Boredom/History
        public async Task<IActionResult> History()
        {
            var history = await _boredomService.GetHistoryAsync(CurrentUserId, 50);
            return View(history);
        }

        // ============================================================
        // AJAX ENDPOINTS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> GetRandom(string? category)
        {
            var activity = await _boredomService.GetRandomActivityAsync(CurrentUserId, category);

            if (activity == null)
                return Json(new { success = false, message = "No activities available." });

            return Json(new { success = true, activity });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start([FromBody] StartSessionRequest request)
        {
            if (request == null || request.ActivityId == Guid.Empty)
                return BadRequest(new { success = false, message = "Invalid activity." });

            try
            {
                var session = await _boredomService.StartSessionAsync(
                    CurrentUserId, request.ActivityId, request.MoodBefore);

                return Json(new { success = true, session });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to start session." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete([FromBody] CompleteSessionRequest request)
        {
            if (request == null || request.SessionId == Guid.Empty)
                return BadRequest(new { success = false, message = "Invalid session." });

            var result = await _boredomService.CompleteSessionAsync(
                request.SessionId, CurrentUserId, request.MoodAfter);

            return Json(new { success = result });
        }

        public class StartSessionRequest
        {
            public Guid ActivityId { get; set; }
            public int? MoodBefore { get; set; }
        }

        public class CompleteSessionRequest
        {
            public Guid SessionId { get; set; }
            public int? MoodAfter { get; set; }
        }
    }
}