using Mindora.Application.DTOs.Payment;

namespace Mindora.Application.Interfaces
{
    public interface IRefundService
    {
        Task<Guid> RequestRefundAsync(Guid userId, CreateRefundRequestRequest request);
        Task<IReadOnlyList<RefundRequestDto>> GetUserRefundRequestsAsync(Guid userId);
        Task<IReadOnlyList<RefundRequestDto>> GetAllRefundRequestsAsync();
        Task<bool> UpdateRefundStatusAsync(Guid adminUserId, UpdateRefundStatusRequest request);
    }
}