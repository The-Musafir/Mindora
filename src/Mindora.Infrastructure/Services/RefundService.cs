using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Payment;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class RefundService : IRefundService
    {
        private readonly MindoraDbContext _context;

        public RefundService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> RequestRefundAsync(Guid userId, CreateRefundRequestRequest request)
        {
            var payment = await _context.Payments.FindAsync(request.PaymentId);
            if (payment == null)
                throw new KeyNotFoundException("Payment not found.");

            var refund = new RefundRequest
            {
                RefundId = Guid.NewGuid(),
                PaymentId = request.PaymentId,
                UserId = userId,
                Reason = request.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.RefundRequests.Add(refund);
            await _context.SaveChangesAsync();
            return refund.RefundId;
        }

        public async Task<IReadOnlyList<RefundRequestDto>> GetUserRefundRequestsAsync(Guid userId)
        {
            var refunds = await _context.RefundRequests
                .Where(r => r.UserId == userId)
                .Include(r => r.Payment)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return refunds.Select(r => new RefundRequestDto
            {
                RefundId = r.RefundId,
                PaymentId = r.PaymentId,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                Amount = r.Payment.Amount,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ReviewedAt = r.ReviewedAt
            }).ToList();
        }

        public async Task<IReadOnlyList<RefundRequestDto>> GetAllRefundRequestsAsync()
        {
            var refunds = await _context.RefundRequests
                .Include(r => r.Payment)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return refunds.Select(r => new RefundRequestDto
            {
                RefundId = r.RefundId,
                PaymentId = r.PaymentId,
                UserId = r.UserId,
                UserEmail = r.User.Email,
                Amount = r.Payment.Amount,
                Reason = r.Reason,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                ReviewedAt = r.ReviewedAt
            }).ToList();
        }

        public async Task<bool> UpdateRefundStatusAsync(Guid adminUserId, UpdateRefundStatusRequest request)
        {
            var refund = await _context.RefundRequests.FindAsync(request.RefundId);
            if (refund == null) return false;

            refund.Status = request.Status;
            refund.ReviewedAt = DateTime.UtcNow;
            refund.ReviewedBy = adminUserId;

            var payment = await _context.Payments.FindAsync(refund.PaymentId);
            if (payment != null)
            {
                payment.Status = request.Status == "Approved" ? "Refunded" : "Completed";
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}