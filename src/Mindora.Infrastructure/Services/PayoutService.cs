using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class PayoutService : IPayoutService
    {
        private readonly MindoraDbContext _context;

        public PayoutService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> RequestPayoutAsync(Guid providerUserId, CreatePayoutRequest request)
        {
            // providerUserId is the UserId of the provider; find ProfessionalProvider
            var provider = await _context.ProfessionalProviders
                .FirstOrDefaultAsync(p => p.UserId == providerUserId);

            if (provider == null)
                throw new KeyNotFoundException("Provider profile not found.");

            var payout = new Payout
            {
                PayoutId = Guid.NewGuid(),
                ProviderId = provider.ProviderId,
                Amount = request.Amount,
                Method = request.Method,
                AccountDetails = request.AccountDetails,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            _context.Payouts.Add(payout);
            await _context.SaveChangesAsync();
            return payout.PayoutId;
        }

        public async Task<IReadOnlyList<PayoutDto>> GetProviderPayoutsAsync(Guid providerUserId)
        {
            var provider = await _context.ProfessionalProviders
                .FirstOrDefaultAsync(p => p.UserId == providerUserId);

            if (provider == null)
                return new List<PayoutDto>();

            var payouts = await _context.Payouts
                .Where(p => p.ProviderId == provider.ProviderId)
                .Include(p => p.Provider.User)
                .OrderByDescending(p => p.RequestedAt)
                .ToListAsync();

            return payouts.Select(MapPayoutToDto).ToList();
        }

        public async Task<IReadOnlyList<PayoutDto>> GetAllPayoutsAsync()
        {
            var payouts = await _context.Payouts
                .Include(p => p.Provider.User)
                .OrderByDescending(p => p.RequestedAt)
                .ToListAsync();

            return payouts.Select(MapPayoutToDto).ToList();
        }

        public async Task<bool> UpdatePayoutStatusAsync(Guid adminUserId, UpdatePayoutStatusRequest request)
        {
            var payout = await _context.Payouts.FindAsync(request.PayoutId);
            if (payout == null) return false;

            payout.Status = request.Status;
            payout.ReviewedAt = DateTime.UtcNow;
            payout.ReviewedBy = adminUserId;

            await _context.SaveChangesAsync();
            return true;
        }

        private PayoutDto MapPayoutToDto(Payout payout)
        {
            return new PayoutDto
            {
                PayoutId = payout.PayoutId,
                ProviderId = payout.ProviderId,
                ProviderName = payout.Provider?.User?.UserName ?? payout.Provider?.User?.Email ?? "Provider",
                Amount = payout.Amount,
                Method = payout.Method,
                AccountDetails = payout.AccountDetails,
                Status = payout.Status,
                RequestedAt = payout.RequestedAt,
                ReviewedAt = payout.ReviewedAt
            };
        }
    }
}