using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Assessment;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class AssessmentController : Controller
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================================================
        // GET: /Assessment
        // ============================================================
        public async Task<IActionResult> Index()
        {
            var questionnaires = await _assessmentService.GetAllQuestionnairesAsync();
            return View(questionnaires);
        }

        // ============================================================
        // GET: /Assessment/Details/{id}
        // ============================================================
        public async Task<IActionResult> Details(Guid id)
        {
            var questionnaire = await _assessmentService.GetQuestionnaireByIdAsync(id);
            if (questionnaire == null) return NotFound();
            return View(questionnaire);
        }

        // ============================================================
        // GET: /Assessment/Take/{id}
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Take(Guid id)
        {
            var questionnaire = await _assessmentService.GetQuestionnaireByIdAsync(id);
            if (questionnaire == null) return NotFound();

            ViewBag.QuestionnaireTitle = questionnaire.Title;
            ViewBag.Questions = questionnaire.Questions;
            return View(new TakeAssessmentRequest { QuestionnaireId = id });
        }

        // ============================================================
        // POST: /Assessment/Take/{id}
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Take(TakeAssessmentRequest request)
        {
            if (!ModelState.IsValid)
            {
                var questionnaire = await _assessmentService.GetQuestionnaireByIdAsync(request.QuestionnaireId);
                if (questionnaire == null) return NotFound();
                ViewBag.QuestionnaireTitle = questionnaire.Title;
                ViewBag.Questions = questionnaire.Questions;
                return View(request);
            }

            var result = await _assessmentService.SubmitAssessmentAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Result), new { id = result.UserAssessmentId });
        }

        // ============================================================
        // GET: /Assessment/Result/{id}
        // ============================================================
        public async Task<IActionResult> Result(Guid id)
        {
            var history = await _assessmentService.GetUserAssessmentHistoryAsync(CurrentUserId);
            var assessment = history.FirstOrDefault(h => h.UserAssessmentId == id);
            if (assessment == null) return NotFound();

            ViewBag.AssessmentId = id;
            return View(assessment);
        }

        // ============================================================
        // GET: /Assessment/History
        // ============================================================
        public async Task<IActionResult> History()
        {
            var history = await _assessmentService.GetUserAssessmentHistoryAsync(CurrentUserId);
            return View(history);
        }

        // ============================================================
        // GET: /Assessment/Statistics
        // ============================================================
        public async Task<IActionResult> Statistics()
        {
            // ✅ Correct method name
            var stats = await _assessmentService.GetAssessmentStatisticsAsync(CurrentUserId);
            var history = await _assessmentService.GetUserAssessmentHistoryAsync(CurrentUserId);

            var vm = new AssessmentStatisticsViewModel
            {
                TotalAssessments = stats.TotalAssessments,
                CompletedAssessments = stats.CompletedAssessments,
                InProgressAssessments = stats.InProgressAssessments,
                AverageScore = stats.AverageScore,
                LowRiskCount = stats.LowRiskCount,
                ModerateRiskCount = stats.ModerateRiskCount,
                HighRiskCount = stats.HighRiskCount
            };

            // ============================================================
            // SCORE HISTORY (chronological order)
            // ============================================================
            var sortedHistory = (history ?? new List<UserAssessmentHistoryDto>())
                .OrderBy(h => h.CompletedAt ?? DateTime.MinValue)
                .ToList();

            foreach (var h in sortedHistory)
            {
                vm.ScoreHistory.Add(new AssessmentScorePoint
                {
                    Date = h.CompletedAt ?? DateTime.UtcNow,
                    // ✅ Explicit cast: int? → double
                    Score = (double)(h.TotalScore ?? 0),
                    QuestionnaireTitle = h.QuestionnaireTitle ?? "Assessment"
                });
            }

            // ============================================================
            // TYPE BREAKDOWN
            // ============================================================
            vm.TypeBreakdown = sortedHistory
                .GroupBy(h => h.QuestionnaireTitle ?? "Other")
                .Select(g => new AssessmentTypeCount
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(t => t.Count)
                .Take(6)
                .ToList();

            return View(vm);
        }
    }
}