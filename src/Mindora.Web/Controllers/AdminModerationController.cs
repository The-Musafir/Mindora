using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminModerationController : Controller
    {
        private readonly IAdminModerationService _moderationService;

        public AdminModerationController(IAdminModerationService moderationService)
        {
            _moderationService = moderationService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /AdminModeration
        public async Task<IActionResult> Index()
        {
            var flags = await _moderationService.GetPendingFlagsAsync();
            return View(flags);
        }

        // GET: /AdminModeration/History
        public async Task<IActionResult> History()
        {
            var flags = await _moderationService.GetResolvedFlagsAsync();
            return View(flags);
        }

        // POST: /AdminModeration/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(Guid flagId)
        {
            await _moderationService.ApproveFlagAsync(flagId, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminModeration/Dismiss
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(Guid flagId)
        {
            await _moderationService.DismissFlagAsync(flagId, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }
    }
}