using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Notification;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // ============================
        // NOTIFICATION CENTER
        // ============================

        // GET: /Notification
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(CurrentUserId);
            var unreadCount = await _notificationService.GetUnreadCountAsync(CurrentUserId);
            ViewBag.UnreadCount = unreadCount;
            return View(notifications);
        }

        // POST: /Notification/MarkAsRead/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Notification/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync(CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Notification/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _notificationService.DeleteNotificationAsync(id, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // NOTIFICATION PREFERENCES
        // ============================

        // GET: /Notification/Preferences
        public async Task<IActionResult> Preferences()
        {
            var preference = await _notificationService.GetUserPreferenceAsync(CurrentUserId);
            if (preference == null)
            {
                preference = new NotificationPreferenceDto
                {
                    EmailEnabled = true,
                    PushEnabled = true,
                    InAppEnabled = true,
                    Frequency = "RealTime"
                };
            }
            return View(preference);
        }

        // POST: /Notification/Preferences
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Preferences(NotificationPreferenceDto model)
        {
            var request = new UpdateNotificationPreferenceRequest
            {
                EmailEnabled = model.EmailEnabled,
                PushEnabled = model.PushEnabled,
                InAppEnabled = model.InAppEnabled,
                QuietHoursEnabled = model.QuietHoursEnabled,
                QuietHoursStart = model.QuietHoursStart,
                QuietHoursEnd = model.QuietHoursEnd,
                Frequency = model.Frequency
            };

            await _notificationService.UpdatePreferenceAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // NOTIFICATION TEMPLATES (Admin)
        // ============================

        // GET: /Notification/Templates
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Templates()
        {
            var templates = await _notificationService.GetAllTemplatesAsync();
            return View(templates);
        }

        // GET: /Notification/CreateTemplate
        [Authorize(Roles = "Admin")]
        public IActionResult CreateTemplate() => View(new NotificationTemplateDto());

        // POST: /Notification/CreateTemplate
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTemplate(NotificationTemplateDto model)
        {
            if (!ModelState.IsValid) return View(model);
            await _notificationService.CreateTemplateAsync(model);
            return RedirectToAction(nameof(Templates));
        }

        // POST: /Notification/ToggleTemplate/{id}
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTemplate(Guid id, bool isActive)
        {
            await _notificationService.ToggleTemplateActiveAsync(id, isActive);
            return RedirectToAction(nameof(Templates));
        }
    }
}