using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminNotificationManagementController : Controller
    {
        private readonly IAdminNotificationManagementService _notificationService;

        public AdminNotificationManagementController(IAdminNotificationManagementService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: /AdminNotificationManagement
        public async Task<IActionResult> Index()
        {
            var data = await _notificationService.GetManagementDataAsync();
            return View(data);
        }

        // POST: /AdminNotificationManagement/CreateTemplate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(NotificationTemplateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TemplateKey) || string.IsNullOrWhiteSpace(dto.Subject))
                return RedirectToAction(nameof(Index));

            await _notificationService.CreateTemplateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminNotificationManagement/ToggleTemplate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTemplate(Guid templateId, bool isActive)
        {
            await _notificationService.ToggleTemplateAsync(templateId, isActive);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminNotificationManagement/Broadcast
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Broadcast(BroadcastNotificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Body))
                return RedirectToAction(nameof(Index));

            await _notificationService.BroadcastAsync(request);
            TempData["BroadcastSuccess"] = "Broadcast sent successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}