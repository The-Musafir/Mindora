using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Follow;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class FollowService : IFollowService
    {
        private readonly MindoraDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        public FollowService(
            MindoraDbContext context,
            INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        public async Task<bool> FollowAsync(Guid followerId, Guid followingId)
        {
            if (followerId == followingId) return false;
            if (followerId == Guid.Empty || followingId == Guid.Empty) return false;

            // Already following?
            var exists = await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (exists) return false;

            // Verify target user exists and is active
            var targetExists = await _context.Users
                .AnyAsync(u => u.Id == followingId && !u.IsDeleted && u.IsActive);

            if (!targetExists) return false;

            var follow = new UserFollow
            {
                FollowId = Guid.NewGuid(),
                FollowerId = followerId,
                FollowingId = followingId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserFollows.Add(follow);
            await _context.SaveChangesAsync();

            // Send notification
            await SendFollowNotificationAsync(followerId, followingId);

            return true;
        }

        public async Task<bool> UnfollowAsync(Guid followerId, Guid followingId)
        {
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);

            if (follow == null) return false;

            _context.UserFollows.Remove(follow);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followingId)
        {
            if (followerId == Guid.Empty || followingId == Guid.Empty) return false;

            return await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FollowingId == followingId);
        }

        public async Task<int> GetFollowerCountAsync(Guid userId)
        {
            return await _context.UserFollows
                .Where(f => f.FollowingId == userId)
                .CountAsync();
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .CountAsync();
        }

        public async Task<IReadOnlyList<FollowUserDto>> GetFollowersAsync(Guid userId, int limit = 50)
        {
            return await _context.UserFollows
                .Where(f => f.FollowingId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Take(limit)
                .Select(f => new FollowUserDto
                {
                    UserId = f.Follower.Id,
                    DisplayName = f.Follower.Profile != null
                        ? f.Follower.Profile.DisplayName
                        : (f.Follower.UserName ?? "User"),
                    AvatarUrl = f.Follower.Profile != null ? f.Follower.Profile.AvatarUrl : null,
                    Bio = f.Follower.Profile != null ? f.Follower.Profile.Bio : null,
                    Role = "Member",
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<FollowUserDto>> GetFollowingAsync(Guid userId, int limit = 50)
        {
            return await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Take(limit)
                .Select(f => new FollowUserDto
                {
                    UserId = f.Following.Id,
                    DisplayName = f.Following.Profile != null
                        ? f.Following.Profile.DisplayName
                        : (f.Following.UserName ?? "User"),
                    AvatarUrl = f.Following.Profile != null ? f.Following.Profile.AvatarUrl : null,
                    Bio = f.Following.Profile != null ? f.Following.Profile.Bio : null,
                    Role = "Member",
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        // ============================================================
        // Private: Send follow notification
        // ============================================================
        private async Task SendFollowNotificationAsync(Guid followerId, Guid followingId)
        {
            try
            {
                var follower = await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Profile)
                    .FirstOrDefaultAsync(u => u.Id == followerId);

                if (follower == null) return;

                var followerName = follower.Profile?.DisplayName
                    ?? follower.UserName
                    ?? "Someone";

                var notification = new Mindora.Application.DTOs.Notification.RealtimeNotificationDto
                {
                    Title = "New Follower",
                    Body = $"{followerName} started following you",
                    Type = "Follow",
                    Channel = "InApp",
                    Severity = "Info",
                    Icon = "fa-user-plus",
                    ActionUrl = $"/u/{followerId}",
                    ReferenceId = followerId,
                    IsSoundEnabled = false
                };

                await _dispatcher.DispatchAsync(followingId, notification);
            }
            catch
            {
                
            }
        }
    }
}