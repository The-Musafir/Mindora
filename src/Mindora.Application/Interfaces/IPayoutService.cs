using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IPayoutService
    {
        Task<Guid> RequestPayoutAsync(Guid providerUserId, CreatePayoutRequest request);
        Task<IReadOnlyList<PayoutDto>> GetProviderPayoutsAsync(Guid providerUserId);
        Task<IReadOnlyList<PayoutDto>> GetAllPayoutsAsync();
        Task<bool> UpdatePayoutStatusAsync(Guid adminUserId, UpdatePayoutStatusRequest request);
    }
}