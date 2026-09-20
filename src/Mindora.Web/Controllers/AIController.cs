using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.AI;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class AIController : Controller
    {
        private readonly IAIWellnessService _aiWellnessService;

        public AIController(IAIWellnessService aiWellnessService)
        {
            _aiWellnessService = aiWellnessService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================
        // SESSION LIST & CREATE
        // ============================

        // GET: /AI
        public async Task<IActionResult> Index()
        {
            var sessions = await _aiWellnessService.GetUserSessionsAsync(CurrentUserId);
            return View(sessions);
        }

        // GET: /AI/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /AI/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? initialContext)
        {
            var session = await _aiWellnessService.CreateSessionAsync(CurrentUserId, initialContext);
            return RedirectToAction(nameof(Chat), new { id = session.SessionId });
        }

        // ============================
        // CHAT INTERFACE
        // ============================

        // GET: /AI/Chat/{id}
        [HttpGet]
        public async Task<IActionResult> Chat(Guid id)
        {
            var session = await _aiWellnessService.GetSessionByIdAsync(id);
            if (session == null || session.UserId != CurrentUserId)
                return NotFound();

            return View(session);
        }

        // POST: /AI/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(SendMessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return RedirectToAction(nameof(Chat), new { id = request.SessionId });

            var response = await _aiWellnessService.SendMessageAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Chat), new { id = request.SessionId });
        }

        // POST: /AI/EndSession/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EndSession(Guid id)
        {
            await _aiWellnessService.EndSessionAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // FEEDBACK
        // ============================

        // POST: /AI/AddFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(CreateAIFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Chat), new { id = request.SessionId });

            await _aiWellnessService.AddFeedbackAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Chat), new { id = request.SessionId });
        }

        // ============================
        // WELLNESS SCORE & RISK PREDICTION
        // ============================

        // GET: /AI/Wellness
        public async Task<IActionResult> Wellness()
        {
            var scores = await _aiWellnessService.GetUserWellnessScoresAsync(CurrentUserId);
            ViewBag.RiskPredictions = await _aiWellnessService.GetUserRiskPredictionsAsync(CurrentUserId);
            return View(scores);
        }

        // POST: /AI/CalculateWellnessScore
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CalculateWellnessScore(CalculateWellnessScoreRequest request)
        {
            request.UserId = CurrentUserId;
            await _aiWellnessService.CalculateAndSaveWellnessScoreAsync(request);
            return RedirectToAction(nameof(Wellness));
        }

        // ============================
        // PROMPT TEMPLATES (Admin)
        // ============================

        // GET: /AI/Templates
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Templates()
        {
            var templates = await _aiWellnessService.GetAllPromptTemplatesAsync();
            return View(templates);
        }

        // POST: /AI/CreateTemplate
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(AIPromptTemplateDto dto)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Templates));

            await _aiWellnessService.CreatePromptTemplateAsync(dto);
            return RedirectToAction(nameof(Templates));
        }

        // POST: /AI/ToggleTemplate
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTemplate(Guid templateId, bool isActive)
        {
            await _aiWellnessService.TogglePromptTemplateAsync(templateId, isActive);
            return RedirectToAction(nameof(Templates));
        }
    }
}