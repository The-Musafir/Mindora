using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class UserAnalyticsController : Controller
    {
        private readonly IUserAnalyticsService _userAnalyticsService;

        public UserAnalyticsController(IUserAnalyticsService userAnalyticsService)
        {
            _userAnalyticsService = userAnalyticsService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /UserAnalytics
        public async Task<IActionResult> Index()
        {
            var analytics = await _userAnalyticsService.GetUserAnalyticsAsync(CurrentUserId);
            return View(analytics);
        }
    }
}