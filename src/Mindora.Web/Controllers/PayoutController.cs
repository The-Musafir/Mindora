using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using System.Security.Claims;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class PayoutController : Controller
    {
        private readonly IPayoutService _payoutService;

        public PayoutController(IPayoutService payoutService)
        {
            _payoutService = payoutService;
        }

        private Guid CurrentUserId => Guid.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

        // GET: /Payout/MyPayouts (Provider)
        public async Task<IActionResult> MyPayouts()
        {
            var payouts = await _payoutService.GetProviderPayoutsAsync(CurrentUserId);
            return View(payouts);
        }

        // POST: /Payout/Request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPayout(CreatePayoutRequest request)
        {
            await _payoutService.RequestPayoutAsync(CurrentUserId, request);
            return RedirectToAction(nameof(MyPayouts));
        }

        // GET: /Payout/Admin (Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var payouts = await _payoutService.GetAllPayoutsAsync();
            return View(payouts);
        }

        // POST: /Payout/UpdateStatus (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(UpdatePayoutStatusRequest request)
        {
            await _payoutService.UpdatePayoutStatusAsync(CurrentUserId, request);
            return RedirectToAction(nameof(Index));
        }
    }
}