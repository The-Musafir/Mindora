using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IAIWellnessService _aiService;

        public ChatController(IAIWellnessService aiService)
        {
            _aiService = aiService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================================================
        // VIEWS
        // ============================================================

        /// <summary>
        /// GET: /Chat
        /// Redirects to the most recent session (or creates a new one).
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var sessions = await _aiService.GetUserSessionsAsync(CurrentUserId);
            var latest = sessions.OrderByDescending(s => s.StartedAt).FirstOrDefault();

            if (latest == null)
            {
                latest = await _aiService.CreateSessionAsync(CurrentUserId);
            }

            return RedirectToAction(nameof(Session), new { id = latest.SessionId });
        }

        /// <summary>
        /// GET: /Chat/Session/{id}
        /// Chat window with sidebar of all sessions.
        /// </summary>
        public async Task<IActionResult> Session(Guid id)
        {
            var session = await _aiService.GetSessionByIdAsync(id);

            if (session == null || session.UserId != CurrentUserId)
                return NotFound();

            var sessions = await _aiService.GetUserSessionsAsync(CurrentUserId);
            ViewBag.Sessions = sessions.OrderByDescending(s => s.StartedAt).ToList();
            ViewBag.ActiveSessionId = id;

            return View(session);
        }

        /// <summary>
        /// POST: /Chat/NewSession
        /// Creates a fresh AI session and redirects.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewSession()
        {
            var session = await _aiService.CreateSessionAsync(CurrentUserId);
            return RedirectToAction(nameof(Session), new { id = session.SessionId });
        }

        // ============================================================
        // AJAX — DELETE / RENAME SESSION
        // ============================================================

        [HttpPost]
        public async Task<IActionResult> DeleteSession([FromBody] SessionActionRequest request)
        {
            if (request == null || request.SessionId == Guid.Empty)
                return BadRequest(new { error = "Invalid session" });

            var ok = await _aiService.DeleteSessionAsync(request.SessionId, CurrentUserId);

            if (!ok) return NotFound(new { error = "Session not found" });

            return Ok(new { deleted = true, sessionId = request.SessionId });
        }

        [HttpPost]
        public async Task<IActionResult> RenameSession([FromBody] RenameSessionRequest request)
        {
            if (request == null || request.SessionId == Guid.Empty)
                return BadRequest(new { error = "Invalid session" });

            var ok = await _aiService.RenameSessionAsync(
                request.SessionId, CurrentUserId, request.Title);

            if (!ok) return NotFound(new { error = "Session not found" });

            return Ok(new { renamed = true, sessionId = request.SessionId, title = request.Title });
        }
    }

    public class SessionActionRequest
    {
        public Guid SessionId { get; set; }
    }

    public class RenameSessionRequest
    {
        public Guid SessionId { get; set; }
        public string? Title { get; set; }
    }
}