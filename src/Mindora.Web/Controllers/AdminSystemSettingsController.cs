using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminSystemSettingsController : Controller
    {
        private readonly IAdminSystemSettingsService _systemSettingsService;

        public AdminSystemSettingsController(IAdminSystemSettingsService systemSettingsService)
        {
            _systemSettingsService = systemSettingsService;
        }

        // GET: /AdminSystemSettings
        public async Task<IActionResult> Index()
        {
            var settings = await _systemSettingsService.GetSettingsAsync();
            return View(settings);
        }

        // POST: /AdminSystemSettings/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AdminSystemSettingsDto dto)
        {
            await _systemSettingsService.UpdateSettingsAsync(dto);
            TempData["SettingsSaved"] = "Settings saved successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}