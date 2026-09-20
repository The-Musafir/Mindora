using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IAdminPaymentManagementService
    {
        Task<AdminPaymentManagementDto> GetManagementDataAsync();
        Task<IReadOnlyList<PaymentResponseDto>> GetAllPaymentsAsync();
        Task<IReadOnlyList<RefundRequestDto>> GetAllRefundsAsync();
        Task<IReadOnlyList<PayoutDto>> GetAllPayoutsAsync();
        Task<IReadOnlyList<CouponDto>> GetAllCouponsAsync();
        Task<bool> UpdateRefundStatusAsync(Guid refundId, string status, Guid adminUserId);
        Task<bool> UpdatePayoutStatusAsync(Guid payoutId, string status, Guid adminUserId);
        Task<bool> ToggleCouponActiveAsync(Guid couponId, bool isActive);
    }
}