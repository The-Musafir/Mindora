using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class PresenceController : Controller
    {
        private readonly IConnectionManager _connectionManager;

        public PresenceController(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

     
        [HttpGet]
        public IActionResult IsOnline(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { error = "userId is required" });

            var isOnline = _connectionManager.IsOnline(userId);
            return Json(new { userId, isOnline });
        }

      
        [HttpGet]
        public IActionResult OnlineUsers()
        {
            var ids = _connectionManager.GetOnlineUserIds();
            return Json(new
            {
                count = ids.Count,
                userIds = ids
            });
        }

       
        [HttpGet]
        public IActionResult Count()
        {
            return Json(new { count = _connectionManager.GetOnlineUserCount() });
        }
    }
}