using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Boredom;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class BoredomService : IBoredomService
    {
        private readonly MindoraDbContext _context;

        public BoredomService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<BoredomActivityDto?> GetRandomActivityAsync(Guid userId, string? category = null)
        {
            var query = _context.BoredomRecoveryActivities
                .Where(a => a.IsActive);

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(a => a.Category == category);
            }

            // Get activity IDs
            var ids = await query.Select(a => a.ActivityId).ToListAsync();
            if (ids.Count == 0) return null;

            // Pick random
            var random = new Random();
            var randomId = ids[random.Next(ids.Count)];

            // Fetch full activity
            var activity = await _context.BoredomRecoveryActivities
                .Where(a => a.ActivityId == randomId)
                .Select(a => new BoredomActivityDto
                {
                    ActivityId = a.ActivityId,
                    Title = a.Title,
                    Description = a.Description,
                    Category = a.Category,
                    DurationMinutes = a.DurationMinutes
                })
                .FirstOrDefaultAsync();

            return activity;
        }

        public async Task<BoredomSessionDto> StartSessionAsync(Guid userId, Guid activityId, int? moodBefore)
        {
            var activity = await _context.BoredomRecoveryActivities
                .FirstOrDefaultAsync(a => a.ActivityId == activityId);

            if (activity == null)
                throw new KeyNotFoundException("Activity not found.");

            var session = new BoredomRecoverySession
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                ActivityId = activityId,
                StartedAt = DateTime.UtcNow,
                MoodBefore = moodBefore
            };

            _context.BoredomRecoverySessions.Add(session);
            await _context.SaveChangesAsync();

            return new BoredomSessionDto
            {
                SessionId = session.SessionId,
                ActivityId = activity.ActivityId,
                ActivityTitle = activity.Title,
                Category = activity.Category,
                StartedAt = session.StartedAt,
                MoodBefore = session.MoodBefore
            };
        }

        public async Task<bool> CompleteSessionAsync(Guid sessionId, Guid userId, int? moodAfter)
        {
            var session = await _context.BoredomRecoverySessions
                .FirstOrDefaultAsync(s => s.SessionId == sessionId && s.UserId == userId);

            if (session == null) return false;
            if (session.EndedAt.HasValue) return false;

            session.EndedAt = DateTime.UtcNow;
            session.MoodAfter = moodAfter;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IReadOnlyList<BoredomSessionDto>> GetHistoryAsync(Guid userId, int limit = 20)
        {
            return await _context.BoredomRecoverySessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.StartedAt)
                .Take(limit)
                .Select(s => new BoredomSessionDto
                {
                    SessionId = s.SessionId,
                    ActivityId = s.ActivityId,
                    ActivityTitle = s.Activity.Title,
                    Category = s.Activity.Category,
                    StartedAt = s.StartedAt,
                    EndedAt = s.EndedAt,
                    MoodBefore = s.MoodBefore,
                    MoodAfter = s.MoodAfter
                })
                .ToListAsync();
        }

        public async Task<IReadOnlyList<BoredomCategoryDto>> GetCategoriesAsync()
        {
            return await _context.BoredomRecoveryActivities
                .Where(a => a.IsActive)
                .GroupBy(a => a.Category)
                .Select(g => new BoredomCategoryDto
                {
                    Name = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<BoredomStatsDto> GetStatsAsync(Guid userId)
        {
            var sessions = await _context.BoredomRecoverySessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Activity)
                .ToListAsync();

            var completed = sessions.Where(s => s.EndedAt.HasValue).ToList();

            var moodImprovements = completed
                .Where(s => s.MoodBefore.HasValue && s.MoodAfter.HasValue)
                .Select(s => s.MoodAfter!.Value - s.MoodBefore!.Value)
                .ToList();

            var favoriteCategory = completed
                .GroupBy(s => s.Activity.Category)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            return new BoredomStatsDto
            {
                TotalSessions = sessions.Count,
                CompletedSessions = completed.Count,
                AverageMoodImprovement = moodImprovements.Any()
                    ? Math.Round(moodImprovements.Average(), 2)
                    : null,
                FavoriteCategory = favoriteCategory
            };
        }
    }
}