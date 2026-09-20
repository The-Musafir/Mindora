using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProfessionalController : Controller
    {
        private readonly IAdminProfessionalService _professionalService;

        public AdminProfessionalController(IAdminProfessionalService professionalService)
        {
            _professionalService = professionalService;
        }

        // GET: /AdminProfessional
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var professionals = await _professionalService.GetAllProfessionalsAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
            return View(professionals);
        }

        // GET: /AdminProfessional/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var professional = await _professionalService.GetProfessionalByIdAsync(id);
            if (professional == null) return NotFound();
            return View(professional);
        }

        // POST: /AdminProfessional/ToggleVerification
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVerification(Guid providerId, bool isVerified)
        {
            await _professionalService.ToggleVerificationAsync(providerId, isVerified);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminProfessional/ToggleActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(Guid providerId, bool isActive)
        {
            await _professionalService.ToggleActiveAsync(providerId, isActive);
            return RedirectToAction(nameof(Index));
        }
    }
}