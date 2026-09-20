using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;

namespace Mindora.Web.Controllers
{
    [Authorize]
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService) => _couponService = couponService;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ApplyCouponRequest request)
        {
            var result = await _couponService.ApplyCouponAsync(request);
            // Implementation depends on UI flow: could return JSON for AJAX, or ViewBag for form
            return Json(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var coupons = await _couponService.GetAllCouponsAsync();
            return View(coupons);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View(new CouponDto());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CouponDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _couponService.CreateCouponAsync(dto);
            return RedirectToAction(nameof(Index));
        }
    }
}