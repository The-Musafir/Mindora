using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class CouponService : ICouponService
    {
        private readonly MindoraDbContext _context;

        public CouponService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<ApplyCouponResponse> ApplyCouponAsync(ApplyCouponRequest request)
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code == request.Code && c.IsActive);

            if (coupon == null)
                return new ApplyCouponResponse { IsValid = false, Message = "Invalid or inactive coupon." };

            if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate < DateTime.UtcNow)
                return new ApplyCouponResponse { IsValid = false, Message = "Coupon expired." };

            if (coupon.UsageLimit > 0 && coupon.UsageCount >= coupon.UsageLimit)
                return new ApplyCouponResponse { IsValid = false, Message = "Coupon usage limit reached." };

            decimal discount = coupon.DiscountType == "Percentage"
                ? request.OrderAmount * (coupon.DiscountValue / 100)
                : coupon.DiscountValue;

            decimal final = Math.Max(0, request.OrderAmount - discount);

            return new ApplyCouponResponse
            {
                IsValid = true,
                Message = "Coupon applied successfully.",
                DiscountAmount = discount,
                FinalAmount = final
            };
        }

        public async Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync()
        {
            var coupons = await _context.Coupons.ToListAsync();
            return coupons.Select(c => new CouponDto
            {
                CouponId = c.CouponId,
                Code = c.Code,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue,
                ExpiryDate = c.ExpiryDate,
                UsageLimit = c.UsageLimit,
                IsActive = c.IsActive
            }).ToList();
        }

        public async Task<Guid> CreateCouponAsync(CouponDto dto)
        {
            var coupon = new Coupon
            {
                CouponId = Guid.NewGuid(),
                Code = dto.Code,
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                ExpiryDate = dto.ExpiryDate,
                UsageLimit = dto.UsageLimit,
                IsActive = dto.IsActive,
                UsageCount = 0,
                CreatedAt = DateTime.UtcNow
            };
            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return coupon.CouponId;
        }

        public async Task<bool> ToggleCouponAsync(Guid couponId, bool isActive)
        {
            var coupon = await _context.Coupons.FindAsync(couponId);
            if (coupon == null) return false;
            coupon.IsActive = isActive;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}