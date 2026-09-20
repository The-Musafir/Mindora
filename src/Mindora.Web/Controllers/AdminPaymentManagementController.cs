using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPaymentManagementController : Controller
    {
        private readonly IAdminPaymentManagementService _paymentManagementService;

        public AdminPaymentManagementController(IAdminPaymentManagementService paymentManagementService)
        {
            _paymentManagementService = paymentManagementService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /AdminPaymentManagement
        public async Task<IActionResult> Index()
        {
            var data = await _paymentManagementService.GetManagementDataAsync();
            return View(data);
        }

        // POST: /AdminPaymentManagement/UpdateRefundStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRefundStatus(Guid refundId, string status)
        {
            await _paymentManagementService.UpdateRefundStatusAsync(refundId, status, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminPaymentManagement/UpdatePayoutStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePayoutStatus(Guid payoutId, string status)
        {
            await _paymentManagementService.UpdatePayoutStatusAsync(payoutId, status, CurrentUserId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminPaymentManagement/ToggleCouponActive
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleCouponActive(Guid couponId, bool isActive)
        {
            await _paymentManagementService.ToggleCouponActiveAsync(couponId, isActive);
            return RedirectToAction(nameof(Index));
        }
    }
}