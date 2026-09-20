using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AnalyticsEventController : Controller
    {
        private readonly IAnalyticsEventService _eventService;

        public AnalyticsEventController(IAnalyticsEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: /AnalyticsEvent
        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetRecentEventsAsync(100);
            return View(events);
        }

        // GET: /AnalyticsEvent/ByUser/{userId}
        public async Task<IActionResult> ByUser(Guid userId)
        {
            var events = await _eventService.GetEventsByUserAsync(userId, 100);
            return View("Index", events);
        }

        // GET: /AnalyticsEvent/ByType?eventType=HabitCreated
        public async Task<IActionResult> ByType(string eventType)
        {
            if (string.IsNullOrWhiteSpace(eventType))
                return RedirectToAction(nameof(Index));

            var events = await _eventService.GetEventsByTypeAsync(eventType, 100);
            return View("Index", events);
        }
    }
}