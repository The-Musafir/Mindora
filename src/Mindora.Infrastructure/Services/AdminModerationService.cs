using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.AdminDashboard;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class AdminModerationService : IAdminModerationService
    {
        private readonly MindoraDbContext _context;

        public AdminModerationService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<AdminModerationFlagDto>> GetPendingFlagsAsync()
        {
            var flags = await _context.CommunityModerationFlags
                .Where(f => f.Status == "Pending")
                .Include(f => f.ReportedByUser)
                .Include(f => f.Post)
                .Include(f => f.Comment)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return flags.Select(MapFlagToDto).ToList();
        }

        public async Task<IReadOnlyList<AdminModerationFlagDto>> GetResolvedFlagsAsync()
        {
            var flags = await _context.CommunityModerationFlags
                .Where(f => f.Status != "Pending")
                .Include(f => f.ReportedByUser)
                .Include(f => f.Post)
                .Include(f => f.Comment)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return flags.Select(MapFlagToDto).ToList();
        }

        public async Task<AdminModerationFlagDto?> GetFlagByIdAsync(Guid flagId)
        {
            var flag = await _context.CommunityModerationFlags
                .Include(f => f.ReportedByUser)
                .Include(f => f.Post)
                .Include(f => f.Comment)
                .FirstOrDefaultAsync(f => f.FlagId == flagId);

            return flag == null ? null : MapFlagToDto(flag);
        }

        public async Task<bool> ApproveFlagAsync(Guid flagId, Guid adminUserId)
        {
            var flag = await _context.CommunityModerationFlags.FindAsync(flagId);
            if (flag == null || flag.Status != "Pending")
                return false;

            // Soft delete content if post/comment exists
            if (flag.PostId.HasValue)
            {
                var post = await _context.CommunityPosts.FindAsync(flag.PostId.Value);
                if (post != null)
                {
                    post.IsDeleted = true;
                    post.UpdatedAt = DateTime.UtcNow;
                }
            }

            if (flag.CommentId.HasValue)
            {
                var comment = await _context.CommunityComments.FindAsync(flag.CommentId.Value);
                if (comment != null)
                {
                    comment.IsDeleted = true;
                }
            }

            flag.Status = "ActionTaken";
            flag.CreatedAt = flag.CreatedAt;

            _context.ModerationActions.Add(new ModerationAction
            {
                ActionId = Guid.NewGuid(),
                FlagId = flagId,
                AdminUserId = adminUserId,
                ActionType = "ContentRemoved",
                Reason = "Community guideline violation",
                ActionedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DismissFlagAsync(Guid flagId, Guid adminUserId)
        {
            var flag = await _context.CommunityModerationFlags.FindAsync(flagId);
            if (flag == null || flag.Status != "Pending")
                return false;

            flag.Status = "Reviewed";

            _context.ModerationActions.Add(new ModerationAction
            {
                ActionId = Guid.NewGuid(),
                FlagId = flagId,
                AdminUserId = adminUserId,
                ActionType = "NoAction",
                Reason = "No violation found",
                ActionedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        private AdminModerationFlagDto MapFlagToDto(CommunityModerationFlag flag)
        {
            string contentType = flag.PostId.HasValue ? "Post" : "Comment";
            string snippet = "";

            if (flag.Post != null)
                snippet = flag.Post.Body.Length > 150 ? flag.Post.Body.Substring(0, 150) + "..." : flag.Post.Body;
            else if (flag.Comment != null)
                snippet = flag.Comment.Content.Length > 150 ? flag.Comment.Content.Substring(0, 150) + "..." : flag.Comment.Content;

            return new AdminModerationFlagDto
            {
                FlagId = flag.FlagId,
                PostId = flag.PostId,
                CommentId = flag.CommentId,
                ContentType = contentType,
                ContentSnippet = snippet,
                ReportedByEmail = flag.ReportedByUser?.Email ?? "Unknown",
                Reason = flag.Reason,
                Status = flag.Status,
                CreatedAt = flag.CreatedAt,
                ReviewedAt = flag.Status == "Pending" ? null : flag.CreatedAt,
                ReviewedBy = null
            };
        }
    }
}