using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminContentManagementController : Controller
    {
        private readonly IAdminContentManagementService _contentService;

        public AdminContentManagementController(IAdminContentManagementService contentService)
        {
            _contentService = contentService;
        }

        // GET: /AdminContentManagement
        public async Task<IActionResult> Index()
        {
            var resources = await _contentService.GetAllWellnessResourcesAsync();
            var assessments = await _contentService.GetAllAssessmentsAsync();
            ViewBag.Assessments = assessments;
            return View(resources);
        }

        // POST: /AdminContentManagement/CreateWellnessResource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWellnessResource(AdminWellnessResourceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Url))
                return RedirectToAction(nameof(Index));

            await _contentService.CreateWellnessResourceAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminContentManagement/ToggleWellnessResource
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWellnessResource(Guid resourceId, bool isPublished)
        {
            await _contentService.ToggleWellnessResourceAsync(resourceId, isPublished);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminContentManagement/CreateAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssessment(AdminAssessmentQuestionnaireDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return RedirectToAction(nameof(Index));

            await _contentService.CreateAssessmentAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminContentManagement/ToggleAssessment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAssessment(Guid questionnaireId, bool isActive)
        {
            await _contentService.ToggleAssessmentAsync(questionnaireId, isActive);
            return RedirectToAction(nameof(Index));
        }
    }
}