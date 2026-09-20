using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class FollowController : Controller
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================================================
        // POST: /Follow/Toggle
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(Guid userId)
        {
            if (userId == Guid.Empty || userId == CurrentUserId)
                return Json(new { success = false, message = "Invalid user" });

            var isFollowing = await _followService.IsFollowingAsync(CurrentUserId, userId);

            bool success;
            if (isFollowing)
            {
                success = await _followService.UnfollowAsync(CurrentUserId, userId);
            }
            else
            {
                success = await _followService.FollowAsync(CurrentUserId, userId);
            }

            var newState = await _followService.IsFollowingAsync(CurrentUserId, userId);
            var newCount = await _followService.GetFollowerCountAsync(userId);

            return Json(new
            {
                success,
                isFollowing = newState,
                followerCount = newCount
            });
        }
    }
}