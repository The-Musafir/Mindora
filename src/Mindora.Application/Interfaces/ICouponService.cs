using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface ICouponService
    {
        Task<ApplyCouponResponse> ApplyCouponAsync(ApplyCouponRequest request);
        Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync();
        Task<Guid> CreateCouponAsync(CouponDto dto);
        Task<bool> ToggleCouponAsync(Guid couponId, bool isActive);
    }
}