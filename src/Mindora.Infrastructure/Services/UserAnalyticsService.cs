using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Analytics;
using Mindora.Application.Interfaces;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class UserAnalyticsService : IUserAnalyticsService
    {
        private readonly MindoraDbContext _context;

        public UserAnalyticsService(MindoraDbContext context)
        {
            _context = context;
        }

        public async Task<UserAnalyticsDto> GetUserAnalyticsAsync(Guid userId)
        {
            var now = DateTime.UtcNow;
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var last30Days = now.AddDays(-30);

            // ============ HABITS ============
            var habits = await _context.Habits
                .Where(h => h.UserId == userId)
                .ToListAsync();

            var activeHabits = habits.Count(h => !h.IsArchived);

            var habitLogs = await _context.HabitLogs
                .Where(l => habits.Select(h => h.HabitId).Contains(l.HabitId))
                .ToListAsync();

            var totalHabitLogs = habitLogs.Count;
            var completedHabitLogs = habitLogs.Count(l => l.IsCompleted);
            double habitCompletionRate = totalHabitLogs > 0
                ? (double)completedHabitLogs / totalHabitLogs * 100
                : 0;

            var streaks = await _context.RecoveryStreaks
                .Where(s => habits.Select(h => h.HabitId).Contains(s.HabitId))
                .ToListAsync();

            int longestStreak = streaks.Any() ? streaks.Max(s => s.LongestStreak) : 0;

            var relapses = await _context.RelapseLogs
                .Where(r => habits.Select(h => h.HabitId).Contains(r.HabitId))
                .CountAsync();

            // ============ JOURNAL ============
            var journalEntries = await _context.JournalEntries
                .Where(j => j.UserId == userId && !j.IsDeleted)
                .ToListAsync();

            var totalJournalEntries = journalEntries.Count;
            var journalEntriesThisMonth = journalEntries.Count(j => j.CreatedAt >= monthStart);

            // Mood score from JournalMoods
            var journalIds = journalEntries.Select(j => j.EntryId).ToList();
            var moods = await _context.JournalMoods
                .Where(m => journalIds.Contains(m.EntryId))
                .ToListAsync();

            double averageMood = moods.Any() ? moods.Average(m => m.Intensity) : 0;

            // ============ ASSESSMENTS ============
            var assessments = await _context.UserAssessments
                .Where(a => a.UserId == userId)
                .Include(a => a.Result)
                .ToListAsync();

            var totalAssessments = assessments.Count;
            var averageAssessmentScore = assessments
                .Where(a => a.Result != null)
                .Select(a => (double)a.Result!.TotalScore)
                .DefaultIfEmpty(0)
                .Average();

            var latestAssessment = assessments
                .Where(a => a.Result != null)
                .OrderByDescending(a => a.CompletedAt)
                .FirstOrDefault();

            // ============ AI ============
            var aiSessions = await _context.AIWellnessCoachSessions
                .Where(s => s.UserId == userId)
                .ToListAsync();

            var totalAIConversations = aiSessions.Count;

            var aiSessionIds = aiSessions.Select(s => s.SessionId).ToList();
            var aiMessagesSent = await _context.AIChatMessages
                .Where(m => aiSessionIds.Contains(m.SessionId) && m.Sender == "User")
                .CountAsync();

            // ============ WELLNESS ============
            var latestWellness = await _context.WellnessScores
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CalculatedAt)
                .FirstOrDefaultAsync();

            // ============ TREND DATA (30 দিন) ============
            var moodTrend = new List<DailyMoodTrendDto>();
            var habitTrend = new List<DailyHabitTrendDto>();

            for (int i = 29; i >= 0; i--)
            {
                var day = now.Date.AddDays(-i);
                var nextDay = day.AddDays(1);

                var dayEntries = journalEntries.Where(j => j.EntryDate == DateOnly.FromDateTime(day)).ToList();
                var dayEntryIds = dayEntries.Select(j => j.EntryId).ToList();
                var dayMoods = moods.Where(m => dayEntryIds.Contains(m.EntryId)).ToList();

                moodTrend.Add(new DailyMoodTrendDto
                {
                    Date = day,
                    MoodScore = dayMoods.Any() ? dayMoods.Average(m => m.Intensity) : 0,
                    JournalEntries = dayEntries.Count
                });

                var dayLogs = habitLogs.Where(l => l.LogDate >= day && l.LogDate < nextDay).ToList();
                int dayCompleted = dayLogs.Count(l => l.IsCompleted);
                int dayTotal = dayLogs.Count;

                habitTrend.Add(new DailyHabitTrendDto
                {
                    Date = day,
                    CompletedHabits = dayCompleted,
                    TotalHabits = dayTotal,
                    CompletionRate = dayTotal > 0 ? (double)dayCompleted / dayTotal * 100 : 0
                });
            }

            return new UserAnalyticsDto
            {
                TotalHabits = habits.Count,
                ActiveHabits = activeHabits,
                CompletedHabitLogs = completedHabitLogs,
                TotalHabitLogs = totalHabitLogs,
                HabitCompletionRate = habitCompletionRate,
                CurrentLongestStreak = longestStreak,
                TotalRelapses = relapses,
                TotalJournalEntries = totalJournalEntries,
                JournalEntriesThisMonth = journalEntriesThisMonth,
                TotalAssessmentsTaken = totalAssessments,
                AverageAssessmentScore = averageAssessmentScore,
                LatestRiskLevel = latestAssessment?.Result?.SeverityLevel,
                TotalAIConversations = totalAIConversations,
                AIMessagesSent = aiMessagesSent,
                LatestWellnessScore = latestWellness?.Score ?? 0,
                LatestWellnessCategory = latestWellness?.Category,
                AverageMoodScore = averageMood,
                MoodTrend = moodTrend,
                HabitTrend = habitTrend
            };
        }
    }
}