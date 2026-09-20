using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class RefundController : Controller
    {
        private readonly IRefundService _refundService;

        public RefundController(IRefundService refundService)
        {
            _refundService = refundService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // POST: /Refund/Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestRefund(CreateRefundRequestRequest request)
        {
            await _refundService.RequestRefundAsync(CurrentUserId, request);
            return RedirectToAction(nameof(MyRefunds));
        }

        // GET: /Refund/MyRefunds
        public async Task<IActionResult> MyRefunds()
        {
            var refunds = await _refundService.GetUserRefundRequestsAsync(CurrentUserId);
            return View(refunds);
        }

        // GET: /Refund/Admin (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var refunds = await _refundService.GetAllRefundRequestsAsync();
            return View(refunds);
        }

        // POST: /Refund/UpdateStatus
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdateRefundStatusRequest request)
        {
            await _refundService.UpdateRefundStatusAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }
    }
}