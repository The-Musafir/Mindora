using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Consultation;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class ConsultationController : Controller
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================
        // SESSION LIST & DETAILS
        // ============================

        // GET: /Consultation
        public async Task<IActionResult> Index()
        {
            var sessions = await _consultationService.GetUserSessionsAsync(CurrentUserId);
            return View(sessions);
        }

        // GET: /Consultation/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var session = await _consultationService.GetSessionByIdAsync(id);
            if (session == null) return NotFound();

            ViewBag.Notes = await _consultationService.GetSessionNotesAsync(id);
            ViewBag.Prescriptions = await _consultationService.GetSessionPrescriptionsAsync(id);
            ViewBag.Feedbacks = await _consultationService.GetSessionFeedbacksAsync(id);
            ViewBag.FollowUpPlans = await _consultationService.GetFollowUpPlansAsync(id);
            ViewBag.Reminders = await _consultationService.GetSessionRemindersAsync(id);
            return View(session);
        }

        // ============================
        // CREATE SESSION
        // ============================

        // GET: /Consultation/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateConsultationSessionRequest());
        }

        // POST: /Consultation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateConsultationSessionRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            await _consultationService.CreateSessionAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // UPDATE SESSION STATUS
        // ============================

        // POST: /Consultation/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateConsultationSessionStatusRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            await _consultationService.UpdateSessionStatusAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }

        // ============================
        // NOTES
        // ============================

        // POST: /Consultation/AddNote
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(CreateConsultationNoteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            request.ProviderId = CurrentUserId;
            await _consultationService.AddNoteAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }

        // ============================
        // PRESCRIPTION
        // ============================

        // POST: /Consultation/AddPrescription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(CreateConsultationPrescriptionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            await _consultationService.AddPrescriptionAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }

        // ============================
        // FEEDBACK
        // ============================

        // POST: /Consultation/AddFeedback
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFeedback(CreateConsultationFeedbackRequest request)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            await _consultationService.AddFeedbackAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }

        // ============================
        // FOLLOW-UP PLAN
        // ============================

        // POST: /Consultation/AddFollowUpPlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFollowUpPlan(CreateFollowUpPlanRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Description))
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            await _consultationService.AddFollowUpPlanAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }

        // POST: /Consultation/ToggleFollowUpPlan/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFollowUpPlan(Guid id, bool isCompleted, Guid sessionId)
        {
            await _consultationService.UpdateFollowUpPlanStatusAsync(id, isCompleted);
            return RedirectToAction(nameof(Details), new { id = sessionId });
        }

        // ============================
        // REMINDER
        // ============================

        // POST: /Consultation/AddReminder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReminder(CreateConsultationReminderRequest request)
        {
            if (request.ReminderAt == default)
                return RedirectToAction(nameof(Details), new { id = request.SessionId });

            await _consultationService.AddReminderAsync(request);
            return RedirectToAction(nameof(Details), new { id = request.SessionId });
        }
    }
}