using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Community;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly MindoraDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public CommunityService(MindoraDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        // ============================
        // POST
        // ============================

        public async Task<PostResponse> CreatePostAsync(Guid userId, CreatePostRequest request)
        {
            var post = new CommunityPost
            {
                PostId = Guid.NewGuid(),
                UserId = userId,
                GroupId = request.GroupId,
                Title = request.Title,
                Body = request.Body,
                Category = request.Category,
                IsAnonymous = request.IsAnonymous,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();
            return await MapPostToResponseAsync(post);
        }

        public async Task<PostResponse?> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.CommunityPosts
                .Include(p => p.User)
                .Include(p => p.Group)
                .Include(p => p.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Reactions)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.PostId == postId && !p.IsDeleted);

            return post == null ? null : await MapPostToResponseAsync(post);
        }

        public async Task<IReadOnlyList<PostResponse>> GetPostsAsync(Guid? userId = null, Guid? groupId = null, string? searchTerm = null)
        {
            var query = _context.CommunityPosts
                .Where(p => !p.IsDeleted)
                .Include(p => p.User)
                .Include(p => p.Group)
                .Include(p => p.Comments.Where(c => !c.IsDeleted))
                    .ThenInclude(c => c.User)
                .Include(p => p.Reactions)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(p => p.UserId == userId.Value);

            if (groupId.HasValue)
                query = query.Where(p => p.GroupId == groupId.Value);

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(p => p.Title.Contains(searchTerm) || p.Body.Contains(searchTerm));

            var posts = await query
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = new List<PostResponse>();
            foreach (var post in posts)
                result.Add(await MapPostToResponseAsync(post));

            return result;
        }

        public async Task<PostResponse> UpdatePostAsync(UpdatePostRequest request)
        {
            var post = await _context.CommunityPosts
                .Include(p => p.User)
                .Include(p => p.Group)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.PostId == request.PostId && !p.IsDeleted);

            if (post == null)
                throw new KeyNotFoundException($"Post {request.PostId} not found.");

            post.Title = request.Title;
            post.Body = request.Body;
            post.GroupId = request.GroupId;
            post.Category = request.Category;
            post.IsAnonymous = request.IsAnonymous;
            post.IsPinned = request.IsPinned;
            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await MapPostToResponseAsync(post);
        }

        public async Task<bool> DeletePostAsync(Guid postId, Guid userId)
        {
            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post == null || post.UserId != userId || post.IsDeleted)
                return false;

            post.IsDeleted = true;
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PinPostAsync(Guid postId, bool isPinned)
        {
            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post == null || post.IsDeleted)
                return false;

            post.IsPinned = isPinned;
            post.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // COMMENT
        // ============================

        public async Task<CommentDto> AddCommentAsync(Guid userId, CreateCommentRequest request)
        {
            var comment = new CommunityComment
            {
                CommentId = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = userId,
                Content = request.Content,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityComments.Add(comment);
            await _context.SaveChangesAsync();

            // reload with user
            var fullComment = await _context.CommunityComments
                .Include(c => c.User)
                .Include(c => c.Reactions)
                .FirstAsync(c => c.CommentId == comment.CommentId);

            // ============================================================
            // NOTIFICATION: Notify post owner
            // ============================================================
            await NotifyPostOwnerOnCommentAsync(request.PostId, userId, request.Content);

            return await MapCommentToDtoAsync(fullComment);
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _context.CommunityComments.FindAsync(commentId);
            if (comment == null || comment.UserId != userId || comment.IsDeleted)
                return false;

            comment.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ============================
        // REACTION
        // ============================

        public async Task<bool> ToggleReactionAsync(Guid userId, Guid? postId, Guid? commentId, string reactionType)
        {
            var existing = await _context.CommunityReactions
                .FirstOrDefaultAsync(r => r.UserId == userId &&
                    ((postId.HasValue && r.PostId == postId.Value) ||
                     (commentId.HasValue && r.CommentId == commentId.Value)));

            if (existing != null)
            {
                _context.CommunityReactions.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // reaction removed
            }

            _context.CommunityReactions.Add(new CommunityReaction
            {
                ReactionId = Guid.NewGuid(),
                UserId = userId,
                PostId = postId,
                CommentId = commentId,
                ReactionType = reactionType,
                CreatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // ============================================================
            // NOTIFICATION: Notify post owner on reaction
            // ============================================================
            if (postId.HasValue)
            {
                await NotifyPostOwnerOnReactionAsync(postId.Value, userId, reactionType);
            }

            return true; // reaction added
        }

        // ============================
        // GROUP
        // ============================

        public async Task<GroupResponse> CreateGroupAsync(Guid userId, CreateGroupRequest request)
        {
            var group = new CommunityGroup
            {
                GroupId = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityGroups.Add(group);
            await _context.SaveChangesAsync();

            _context.CommunityGroupMembers.Add(new CommunityGroupMember
            {
                GroupId = group.GroupId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            return await MapGroupToResponseAsync(group);
        }

        public async Task<GroupResponse?> GetGroupByIdAsync(Guid groupId)
        {
            var group = await _context.CommunityGroups
                .Include(g => g.CreatedByUser)
                .Include(g => g.Members)
                .Include(g => g.Posts.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(g => g.GroupId == groupId && !g.IsDeleted);

            return group == null ? null : await MapGroupToResponseAsync(group);
        }

        public async Task<IReadOnlyList<GroupResponse>> GetGroupsAsync()
        {
            var groups = await _context.CommunityGroups
                .Where(g => !g.IsDeleted)
                .Include(g => g.CreatedByUser)
                .Include(g => g.Members)
                .Include(g => g.Posts.Where(p => !p.IsDeleted))
                .ToListAsync();

            var result = new List<GroupResponse>();
            foreach (var group in groups)
                result.Add(await MapGroupToResponseAsync(group));

            return result;
        }

        public async Task<bool> JoinGroupAsync(Guid groupId, Guid userId)
        {
            var exists = await _context.CommunityGroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (exists) return false;

            _context.CommunityGroupMembers.Add(new CommunityGroupMember
            {
                GroupId = groupId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> LeaveGroupAsync(Guid groupId, Guid userId)
        {
            var member = await _context.CommunityGroupMembers
                .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (member == null) return false;

            _context.CommunityGroupMembers.Remove(member);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IReadOnlyList<GroupMemberDto>> GetGroupMembersAsync(Guid groupId)
        {
            var members = await _context.CommunityGroupMembers
                .Where(m => m.GroupId == groupId)
                .Include(m => m.User)
                .ToListAsync();

            return members.Select(m => new GroupMemberDto
            {
                UserId = m.UserId,
                UserName = m.User?.UserName ?? m.User?.Email ?? string.Empty,
                JoinedAt = m.JoinedAt
            }).ToList();
        }

        // ============================
        // MODERATION
        // ============================

        public async Task<Guid> FlagContentAsync(Guid userId, FlagRequest request)
        {
            var flag = new CommunityModerationFlag
            {
                FlagId = Guid.NewGuid(),
                PostId = request.PostId,
                CommentId = request.CommentId,
                ReportedByUserId = userId,
                Reason = request.Reason,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityModerationFlags.Add(flag);
            await _context.SaveChangesAsync();
            return flag.FlagId;
        }

        public async Task<IReadOnlyList<ModerationFlagDto>> GetPendingFlagsAsync()
        {
            var flags = await _context.CommunityModerationFlags
                .Where(f => f.Status == "Pending")
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return flags.Select(f => new ModerationFlagDto
            {
                FlagId = f.FlagId,
                PostId = f.PostId,
                CommentId = f.CommentId,
                Reason = f.Reason,
                Status = f.Status,
                CreatedAt = f.CreatedAt
            }).ToList();
        }

        public async Task<bool> ResolveFlagAsync(Guid flagId, string resolution, Guid adminUserId)
        {
            var flag = await _context.CommunityModerationFlags.FindAsync(flagId);
            if (flag == null || flag.Status != "Pending") return false;

            flag.Status = resolution;

            _context.ModerationActions.Add(new ModerationAction
            {
                ActionId = Guid.NewGuid(),
                FlagId = flagId,
                AdminUserId = adminUserId,
                ActionType = resolution,
                Reason = "Moderator action",
                ActionedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // ============================================================
        // NOTIFICATION HELPERS
        // ============================================================

        private async Task NotifyPostOwnerOnCommentAsync(Guid postId, Guid commenterId, string commentContent)
        {
            try
            {
                var post = await _context.CommunityPosts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PostId == postId);

                if (post == null || post.UserId == commenterId || post.IsAnonymous) return;

                var commenter = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == commenterId);

                var commenterName = commenter?.UserName ?? commenter?.Email ?? "Someone";

                var preview = commentContent.Length > 50
                    ? commentContent.Substring(0, 50) + "..."
                    : commentContent;

                var notification = MindoraNotifications.CommunityNewComment(
                    commenterName, preview, postId);

                await _dispatcher.DispatchAsync(post.UserId, notification);
            }
            catch
            {
                // Notification failure must NEVER break the main flow
            }
        }

        private async Task NotifyPostOwnerOnReactionAsync(Guid postId, Guid reactorId, string reactionType)
        {
            try
            {
                var post = await _context.CommunityPosts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.PostId == postId);

                if (post == null || post.UserId == reactorId || post.IsAnonymous) return;

                var reactor = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == reactorId);

                var reactorName = reactor?.UserName ?? reactor?.Email ?? "Someone";

                var notification = MindoraNotifications.CommunityNewReaction(
                    reactorName, reactionType, postId);

                await _dispatcher.DispatchAsync(post.UserId, notification);
            }
            catch
            {
                // Silent fail
            }
        }

        // ============================
        // PRIVATE MAPPING HELPERS
        // ============================

        private async Task<PostResponse> MapPostToResponseAsync(CommunityPost post)
        {
            var likeCount = post.Reactions?.Count(r => r.ReactionType == "Like") ?? 0;
            var commentCount = post.Comments?.Count(c => !c.IsDeleted) ?? 0;

            var comments = new List<CommentDto>();
            if (post.Comments != null)
            {
                foreach (var comment in post.Comments.Where(c => c.ParentCommentId == null && !c.IsDeleted))
                {
                    comments.Add(await MapCommentToDtoAsync(comment));
                }
            }

            return new PostResponse
            {
                PostId = post.PostId,
                UserId = post.UserId,
                AuthorName = post.IsAnonymous ? "Anonymous" : post.User?.UserName ?? post.User?.Email ?? "User",
                IsAnonymous = post.IsAnonymous,
                GroupId = post.GroupId,
                GroupName = post.Group?.Name,
                Title = post.Title,
                Body = post.Body,
                Category = post.Category,
                IsPinned = post.IsPinned,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = likeCount,
                CommentCount = commentCount,
                Comments = comments
            };
        }

        private async Task<CommentDto> MapCommentToDtoAsync(CommunityComment comment)
        {
            var likeCount = comment.Reactions?.Count(r => r.ReactionType == "Like") ?? 0;

            var replies = new List<CommentDto>();
            if (comment.Replies != null)
            {
                foreach (var reply in comment.Replies.Where(r => !r.IsDeleted))
                {
                    replies.Add(await MapCommentToDtoAsync(reply));
                }
            }

            return new CommentDto
            {
                CommentId = comment.CommentId,
                UserId = comment.UserId,
                AuthorName = comment.User?.UserName ?? comment.User?.Email ?? "User",
                Content = comment.Content,
                ParentCommentId = comment.ParentCommentId,
                CreatedAt = comment.CreatedAt,
                LikeCount = likeCount,
                Replies = replies
            };
        }

        private async Task<GroupResponse> MapGroupToResponseAsync(CommunityGroup group)
        {
            return new GroupResponse
            {
                GroupId = group.GroupId,
                Name = group.Name,
                Description = group.Description,
                CreatedByUserId = group.CreatedByUserId,
                CreatedByName = group.CreatedByUser?.UserName ?? group.CreatedByUser?.Email ?? "User",
                CreatedAt = group.CreatedAt,
                MemberCount = group.Members?.Count ?? 0,
                PostCount = group.Posts?.Count(p => !p.IsDeleted) ?? 0
            };
        }
    }
}