using Microsoft.EntityFrameworkCore;
using Mindora.Application.DTOs.Habit;
using Mindora.Application.Interfaces;
using Mindora.Domain.Entities;
using Mindora.Domain.Enums;
using Mindora.Infrastructure.Notifications;
using Mindora.Infrastructure.Persistence.DbContext;

namespace Mindora.Infrastructure.Services
{
    public class HabitService : IHabitService
    {
        private readonly MindoraDbContext _context;
        private readonly INotificationDispatcher _dispatcher;

        // Streak milestone thresholds — when current streak hits these, notify user
        private static readonly int[] StreakMilestones = { 7, 14, 30, 60, 90, 180, 365 };

        public HabitService(MindoraDbContext context, INotificationDispatcher dispatcher)
        {
            _context = context;
            _dispatcher = dispatcher;
        }

        // ============================
        // CORE CRUD
        // ============================

        public async Task<HabitResponse> CreateHabitAsync(Guid userId, CreateHabitRequest request)
        {
            var habit = new Habit
            {
                HabitId = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                Description = request.Description,
                HabitCategoryId = request.CategoryId,
                Frequency = request.Frequency,
                Priority = request.Priority,
                Difficulty = request.Difficulty,
                Color = request.Color,
                Icon = request.Icon,
                CreatedAt = DateTime.UtcNow
            };

            _context.Habits.Add(habit);
            await _context.SaveChangesAsync();

            return await MapToResponseAsync(habit);
        }

        public async Task<HabitResponse?> GetHabitByIdAsync(Guid habitId)
        {
            var habit = await _context.Habits
                .Include(h => h.HabitCategory)
                .Include(h => h.Goals)
                .Include(h => h.Logs)
                .Include(h => h.Reminders)
                .Include(h => h.Streaks)
                .Include(h => h.Relapses)
                .Include(h => h.Milestones)
                .FirstOrDefaultAsync(h => h.HabitId == habitId);

            return habit == null ? null : await MapToResponseAsync(habit);
        }

        public async Task<IReadOnlyList<HabitResponse>> GetUserHabitsAsync(Guid userId)
        {
            var habits = await _context.Habits
                .Where(h => h.UserId == userId && !h.IsArchived)
                .Include(h => h.HabitCategory)
                .Include(h => h.Goals)
                .Include(h => h.Logs)
                .Include(h => h.Reminders)
                .Include(h => h.Streaks)
                .Include(h => h.Relapses)
                .Include(h => h.Milestones)
                .ToListAsync();

            var result = new List<HabitResponse>();
            foreach (var habit in habits)
                result.Add(await MapToResponseAsync(habit));

            return result;
        }

        public async Task<HabitResponse> UpdateHabitAsync(UpdateHabitRequest request)
        {
            var habit = await _context.Habits
                .FirstOrDefaultAsync(h => h.HabitId == request.HabitId);

            if (habit == null)
                throw new KeyNotFoundException($"Habit with ID {request.HabitId} not found.");

            habit.Name = request.Name;
            habit.Description = request.Description;
            habit.HabitCategoryId = request.CategoryId;
            habit.Frequency = request.Frequency;
            habit.Priority = request.Priority;
            habit.Difficulty = request.Difficulty;
            habit.Color = request.Color;
            habit.Icon = request.Icon;
            habit.IsArchived = request.IsArchived;
            habit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await MapToResponseAsync(habit);
        }

        public async Task<bool> DeleteHabitAsync(Guid habitId)
        {
            var habit = await _context.Habits.FindAsync(habitId);
            if (habit == null) return false;

            _context.Habits.Remove(habit);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ArchiveHabitAsync(Guid habitId)
        {
            var habit = await _context.Habits.FindAsync(habitId);
            if (habit == null) return false;

            habit.IsArchived = true;
            habit.UpdatedAt = DateTime.UtcNow;
            return await _context.SaveChangesAsync() > 0;
        }

        // ============================
        // CATEGORY
        // ============================

        public async Task<IReadOnlyList<HabitCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _context.HabitCategories
                .Select(c => new HabitCategoryDto
                {
                    CategoryId = c.HabitCategoryId,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return categories;
        }

        public async Task<Guid> CreateCategoryAsync(HabitCategoryDto dto)
        {
            var category = new HabitCategory
            {
                HabitCategoryId = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            _context.HabitCategories.Add(category);
            await _context.SaveChangesAsync();
            return category.HabitCategoryId;
        }

        // ============================
        // DAILY LOG / TRACKING
        // ============================

        public async Task<bool> CompleteHabitAsync(Guid habitId, DateTime logDate, string? note = null)
        {
            var habit = await _context.Habits.FindAsync(habitId);
            if (habit == null) return false;

            var existingLog = await _context.HabitLogs
                .FirstOrDefaultAsync(l => l.HabitId == habitId && l.LogDate.Date == logDate.Date);

            if (existingLog == null)
            {
                _context.HabitLogs.Add(new HabitLog
                {
                    HabitLogId = Guid.NewGuid(),
                    HabitId = habitId,
                    LogDate = logDate.Date,
                    IsCompleted = true,
                    Note = note
                });
            }
            else
            {
                existingLog.IsCompleted = true;
                existingLog.Note = note ?? existingLog.Note;
            }

            await UpdateStreakOnCompletionAsync(habitId, logDate.Date);

            var saved = await _context.SaveChangesAsync() > 0;

            // ============================================================
            // NOTIFICATION: Streak milestone reached
            // ============================================================
            if (saved)
            {
                await NotifyStreakMilestoneIfReachedAsync(habit);
            }

            return saved;
        }

        public async Task<HabitLogDto> GetDailyLogAsync(Guid habitId, DateTime logDate)
        {
            var log = await _context.HabitLogs
                .FirstOrDefaultAsync(l => l.HabitId == habitId && l.LogDate.Date == logDate.Date);

            return log == null ? null! : new HabitLogDto
            {
                HabitLogId = log.HabitLogId,
                LogDate = log.LogDate,
                IsCompleted = log.IsCompleted,
                Note = log.Note,
                Reflection = log.Reflection,
                MoodBefore = log.MoodBefore,
                MoodAfter = log.MoodAfter,
                CompletionTimeMinutes = log.CompletionTimeMinutes
            };
        }

        // ============================
        // REMINDER
        // ============================

        public async Task<Guid> AddReminderAsync(Guid habitId, TimeSpan reminderTime)
        {
            var reminder = new HabitReminder
            {
                HabitReminderId = Guid.NewGuid(),
                HabitId = habitId,
                ReminderTime = reminderTime,
                IsEnabled = true
            };

            _context.HabitReminders.Add(reminder);
            await _context.SaveChangesAsync();
            return reminder.HabitReminderId;
        }

        public async Task<bool> ToggleReminderAsync(Guid reminderId)
        {
            var reminder = await _context.HabitReminders.FindAsync(reminderId);
            if (reminder == null) return false;

            reminder.IsEnabled = !reminder.IsEnabled;
            return await _context.SaveChangesAsync() > 0;
        }

        // ============================
        // STREAK & RECOVERY
        // ============================

        public async Task<int> GetCurrentStreakAsync(Guid habitId)
        {
            var streak = await _context.RecoveryStreaks
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.EndDate == null);

            return streak?.CurrentStreak ?? 0;
        }

        public async Task<int> GetLongestStreakAsync(Guid habitId)
        {
            var maxStreak = await _context.RecoveryStreaks
                .Where(s => s.HabitId == habitId)
                .MaxAsync(s => (int?)s.LongestStreak) ?? 0;

            return maxStreak;
        }

        public async Task<RecoveryStreakDto?> GetStreakInfoAsync(Guid habitId)
        {
            var streak = await _context.RecoveryStreaks
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.EndDate == null);

            if (streak == null) return null;

            return new RecoveryStreakDto
            {
                RecoveryStreakId = streak.RecoveryStreakId,
                CurrentStreak = streak.CurrentStreak,
                LongestStreak = streak.LongestStreak,
                StartDate = streak.StartDate,
                EndDate = streak.EndDate
            };
        }

        // ============================
        // RELAPSE
        // ============================

        public async Task<Guid> LogRelapseAsync(Guid habitId, string? reason = null, string? note = null)
        {
            var habit = await _context.Habits.FindAsync(habitId);
            if (habit == null)
                throw new KeyNotFoundException($"Habit with ID {habitId} not found.");

            var relapse = new RelapseLog
            {
                RelapseLogId = Guid.NewGuid(),
                HabitId = habitId,
                RelapseDate = DateTime.UtcNow,
                Reason = reason,
                Note = note
            };

            _context.RelapseLogs.Add(relapse);

            var activeStreak = await _context.RecoveryStreaks
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.EndDate == null);

            if (activeStreak != null)
            {
                activeStreak.EndDate = DateTime.UtcNow;
                activeStreak.CurrentStreak = 0;
            }

            await _context.SaveChangesAsync();

            // ============================================================
            // NOTIFICATION: Relapse logged (supportive, not punitive)
            // ============================================================
            var relapseNotification = MindoraNotifications.HabitRelapseAlert(
                habit.Name, habit.HabitId);

            await _dispatcher.DispatchAsync(habit.UserId, relapseNotification);

            return relapse.RelapseLogId;
        }

        public async Task<IReadOnlyList<RelapseLogDto>> GetRelapsesAsync(Guid habitId)
        {
            var relapses = await _context.RelapseLogs
                .Where(r => r.HabitId == habitId)
                .OrderByDescending(r => r.RelapseDate)
                .Select(r => new RelapseLogDto
                {
                    RelapseLogId = r.RelapseLogId,
                    RelapseDate = r.RelapseDate,
                    Reason = r.Reason,
                    Note = r.Note
                })
                .ToListAsync();

            return relapses;
        }

        // ============================
        // ANALYTICS / STATISTICS
        // ============================

        public async Task<HabitStatisticsDto> GetHabitStatisticsAsync(Guid habitId)
        {
            var logs = await _context.HabitLogs
                .Where(l => l.HabitId == habitId)
                .ToListAsync();

            int totalLogs = logs.Count;
            int completedLogs = logs.Count(l => l.IsCompleted);
            double completionRate = totalLogs > 0 ? (double)completedLogs / totalLogs * 100 : 0;

            int currentStreak = await GetCurrentStreakAsync(habitId);
            int longestStreak = await GetLongestStreakAsync(habitId);

            DateTime today = DateTime.UtcNow.Date;
            var last7Days = logs.Where(l => l.LogDate >= today.AddDays(-6)).ToList();
            var last30Days = logs.Where(l => l.LogDate >= today.AddDays(-29)).ToList();

            double weeklyAvg = last7Days.Count > 0 ? last7Days.Count(l => l.IsCompleted) / 7.0 * 100 : 0;
            double monthlyAvg = last30Days.Count > 0 ? last30Days.Count(l => l.IsCompleted) / 30.0 * 100 : 0;

            int totalRelapses = await _context.RelapseLogs.CountAsync(r => r.HabitId == habitId);

            return new HabitStatisticsDto
            {
                TotalLogs = totalLogs,
                CompletedLogs = completedLogs,
                CompletionRate = completionRate,
                CurrentStreak = currentStreak,
                LongestStreak = longestStreak,
                WeeklyAverage = weeklyAvg,
                MonthlyAverage = monthlyAvg,
                TotalRelapses = totalRelapses
            };
        }

        // ============================
        // PRIVATE HELPERS
        // ============================

        /// <summary>
        /// After a habit completion, checks if the current streak just hit
        /// a milestone threshold. If yes, dispatches a realtime notification.
        /// </summary>
        private async Task NotifyStreakMilestoneIfReachedAsync(Habit habit)
        {
            try
            {
                var activeStreak = await _context.RecoveryStreaks
                    .FirstOrDefaultAsync(s => s.HabitId == habit.HabitId && s.EndDate == null);

                if (activeStreak == null) return;

                var currentStreak = activeStreak.CurrentStreak;

                // Only notify if this is exactly a milestone (not every day)
                if (!StreakMilestones.Contains(currentStreak)) return;

                var notification = MindoraNotifications.HabitStreakMilestone(
                    habit.Name, currentStreak, habit.HabitId);

                await _dispatcher.DispatchAsync(habit.UserId, notification);
            }
            catch
            {
                // Notification failure must NEVER break habit completion flow.
                // Logging handled inside dispatcher.
            }
        }

        private async Task<HabitResponse> MapToResponseAsync(Habit habit)
        {
            var category = habit.HabitCategory != null
                ? new HabitCategoryDto
                {
                    CategoryId = habit.HabitCategory.HabitCategoryId,
                    Name = habit.HabitCategory.Name,
                    Description = habit.HabitCategory.Description
                }
                : null;

            return new HabitResponse
            {
                HabitId = habit.HabitId,
                Name = habit.Name,
                Description = habit.Description,
                CategoryId = habit.HabitCategoryId,
                CategoryName = category?.Name,
                Frequency = habit.Frequency,
                Priority = habit.Priority,
                Difficulty = habit.Difficulty,
                Color = habit.Color,
                Icon = habit.Icon,
                IsArchived = habit.IsArchived,
                CreatedAt = habit.CreatedAt,
                UpdatedAt = habit.UpdatedAt,

                CurrentStreak = habit.Streaks
                    .Where(s => s.EndDate == null)
                    .OrderByDescending(s => s.StartDate)
                    .Select(s => s.CurrentStreak)
                    .FirstOrDefault(),

                CompletionRate = habit.Logs.Any()
                    ? (double)habit.Logs.Count(l => l.IsCompleted) / habit.Logs.Count * 100
                    : 0,

                Goals = habit.Goals.Select(g => new HabitGoalDto
                {
                    GoalId = g.GoalId,
                    TargetValue = g.TargetValue,
                    Unit = g.Unit,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate,
                    IsCompleted = g.IsCompleted
                }).ToList(),
                Logs = habit.Logs.Select(l => new HabitLogDto
                {
                    HabitLogId = l.HabitLogId,
                    LogDate = l.LogDate,
                    IsCompleted = l.IsCompleted,
                    Note = l.Note,
                    Reflection = l.Reflection,
                    MoodBefore = l.MoodBefore,
                    MoodAfter = l.MoodAfter,
                    CompletionTimeMinutes = l.CompletionTimeMinutes
                }).ToList(),
                Reminders = habit.Reminders.Select(r => new HabitReminderDto
                {
                    HabitReminderId = r.HabitReminderId,
                    ReminderTime = r.ReminderTime,
                    IsEnabled = r.IsEnabled
                }).ToList(),
                Streaks = habit.Streaks.Select(s => new RecoveryStreakDto
                {
                    RecoveryStreakId = s.RecoveryStreakId,
                    CurrentStreak = s.CurrentStreak,
                    LongestStreak = s.LongestStreak,
                    StartDate = s.StartDate,
                    EndDate = s.EndDate
                }).ToList(),
                Relapses = habit.Relapses.Select(r => new RelapseLogDto
                {
                    RelapseLogId = r.RelapseLogId,
                    RelapseDate = r.RelapseDate,
                    Reason = r.Reason,
                    Note = r.Note
                }).ToList(),
                Milestones = habit.Milestones.Select(m => new HabitMilestoneDto
                {
                    MilestoneId = m.MilestoneId,
                    Title = m.Title,
                    AchievedAt = m.AchievedAt
                }).ToList()
            };
        }

        private async Task UpdateStreakOnCompletionAsync(Guid habitId, DateTime completionDate)
        {
            var activeStreak = await _context.RecoveryStreaks
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.EndDate == null);

            if (activeStreak == null)
            {
                var newStreak = new RecoveryStreak
                {
                    RecoveryStreakId = Guid.NewGuid(),
                    HabitId = habitId,
                    CurrentStreak = 1,
                    LongestStreak = 1,
                    StartDate = completionDate,
                    EndDate = null
                };
                _context.RecoveryStreaks.Add(newStreak);
            }
            else
            {
                var lastDate = activeStreak.StartDate.AddDays(activeStreak.CurrentStreak - 1);
                if (completionDate.Date == lastDate.Date.AddDays(1))
                {
                    activeStreak.CurrentStreak++;
                    if (activeStreak.CurrentStreak > activeStreak.LongestStreak)
                        activeStreak.LongestStreak = activeStreak.CurrentStreak;
                }
                else if (completionDate.Date > lastDate.Date)
                {
                    activeStreak.EndDate = lastDate;
                    var newStreak = new RecoveryStreak
                    {
                        RecoveryStreakId = Guid.NewGuid(),
                        HabitId = habitId,
                        CurrentStreak = 1,
                        LongestStreak = activeStreak.LongestStreak,
                        StartDate = completionDate,
                        EndDate = null
                    };
                    _context.RecoveryStreaks.Add(newStreak);
                }
            }
        }
    }
}