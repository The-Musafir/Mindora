using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Home;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class LandingPageService : ILandingPageService
    {
        private readonly MindoraDbContext _context;

        public LandingPageService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<HomeViewModel> BuildHomeViewModelAsync(Guid? currentUserId)
        {
            var vm = new HomeViewModel();

            // ============================================================
            // STATISTICS (Public)
            // ============================================================
            vm.TotalMembers = await _context.Users
                .Where(u => !u.IsDeleted && u.IsActive)
                .CountAsync();

            vm.TotalHabitsCompleted = await _context.HabitLogs
                .Where(h => h.IsCompleted)
                .CountAsync();

            vm.TotalSessions = await _context.Appointments.CountAsync();

            // Positive progress: users with at least one 7+ day streak
            var totalUsersWithHabits = await _context.Habits
                .Select(h => h.UserId).Distinct().CountAsync();

            var usersWithGoodStreaks = await _context.RecoveryStreaks
                .Where(s => s.LongestStreak >= 7)
                .Select(s => s.HabitId)
                .Distinct()
                .Join(_context.Habits, habitId => habitId, h => h.HabitId, (habitId, h) => h.UserId)
                .Distinct()
                .CountAsync();

            vm.PositiveProgressRate = totalUsersWithHabits > 0
                ? (int)Math.Round((double)usersWithGoodStreaks / totalUsersWithHabits * 100)
                : 92;

            // ============================================================
            // TOP PROVIDERS (Public)
            // ============================================================
            vm.TopProviders = await _context.ProfessionalProviders
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Reviews.Any()
                    ? p.Reviews.Average(r => (double)r.Rating)
                    : 0)
                .ThenByDescending(p => p.Reviews.Count)
                .Take(6)
                .Select(p => new LandingProviderDto
                {
                    ProviderId = p.ProviderId,
                    Name = p.User.UserName ?? "Provider",
                    Specialty = p.SpecialtyMappings
                        .Select(m => m.Specialty.Name)
                        .FirstOrDefault(),
                    Rating = p.Reviews.Any()
                        ? p.Reviews.Average(r => (double)r.Rating)
                        : null,
                    ReviewCount = p.Reviews.Count,
                    YearsOfExperience = p.YearsOfExperience
                })
                .ToListAsync();

            // ============================================================
            // TESTIMONIALS (Public — top rated reviews)
            // ============================================================
            vm.Testimonials = await _context.Reviews
                .Where(r => r.Rating >= 4 && !string.IsNullOrEmpty(r.Comment))
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.CreatedAt)
                .Take(3)
                .Select(r => new LandingTestimonialDto
                {
                    ReviewerName = r.User.UserName ?? "User",
                    Comment = r.Comment ?? "",
                    Rating = r.Rating
                })
                .ToListAsync();

            // ============================================================
            // TODAY'S PROGRESS (only if logged in)
            // ============================================================
            if (currentUserId.HasValue)
            {
                vm.IsUserLoggedIn = true;
                var userId = currentUserId.Value;
                var today = DateTime.UtcNow.Date;

                // Total active habits
                var habitIds = await _context.Habits
                    .Where(h => h.UserId == userId && !h.IsArchived)
                    .Select(h => h.HabitId)
                    .ToListAsync();

                vm.TodayHabitsTotal = habitIds.Count;

                // Completed today
                vm.TodayHabitsDone = await _context.HabitLogs
                    .Where(l => habitIds.Contains(l.HabitId)
                             && l.LogDate.Date == today
                             && l.IsCompleted)
                    .CountAsync();

                // Daily goal %
                vm.DailyGoalPercent = (vm.TodayHabitsTotal > 0)
                    ? (int)Math.Round((double)vm.TodayHabitsDone.Value / vm.TodayHabitsTotal.Value * 100)
                    : 0;

                // Latest wellness score
                var latestWellness = await _context.WellnessScores
                    .Where(w => w.UserId == userId)
                    .OrderByDescending(w => w.CalculatedAt)
                    .FirstOrDefaultAsync();

                if (latestWellness != null)
                {
                    vm.LatestMoodScore = latestWellness.Score;
                }

                // Current streak
                vm.CurrentStreak = await _context.RecoveryStreaks
                    .Where(s => s.HabitId != Guid.Empty && s.EndDate == null)
                    .Join(_context.Habits, s => s.HabitId, h => h.HabitId, (s, h) => new { s, h })
                    .Where(x => x.h.UserId == userId)
                    .OrderByDescending(x => x.s.CurrentStreak)
                    .Select(x => x.s.CurrentStreak)
                    .FirstOrDefaultAsync();
            }

            return vm;
        }
    }
}