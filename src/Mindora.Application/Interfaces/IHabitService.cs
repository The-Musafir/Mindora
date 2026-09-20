using Mindora.Application.DTOs.Habit;

namespace Mindora.Application.Interfaces
{
    public interface IHabitService
    {
        // Core CRUD
        Task<HabitResponse> CreateHabitAsync(Guid userId, CreateHabitRequest request);
        Task<HabitResponse?> GetHabitByIdAsync(Guid habitId);
        Task<IReadOnlyList<HabitResponse>> GetUserHabitsAsync(Guid userId);
        Task<HabitResponse> UpdateHabitAsync(UpdateHabitRequest request);
        Task<bool> DeleteHabitAsync(Guid habitId);
        Task<bool> ArchiveHabitAsync(Guid habitId);

        // Category
        Task<IReadOnlyList<HabitCategoryDto>> GetAllCategoriesAsync();
        Task<Guid> CreateCategoryAsync(HabitCategoryDto dto);

        // Daily Log / Tracking
        Task<bool> CompleteHabitAsync(Guid habitId, DateTime logDate, string? note = null);
        Task<HabitLogDto> GetDailyLogAsync(Guid habitId, DateTime logDate);

        // Reminder
        Task<Guid> AddReminderAsync(Guid habitId, TimeSpan reminderTime);
        Task<bool> ToggleReminderAsync(Guid reminderId);

        // Streak & Recovery
        Task<int> GetCurrentStreakAsync(Guid habitId);
        Task<int> GetLongestStreakAsync(Guid habitId);
        Task<RecoveryStreakDto?> GetStreakInfoAsync(Guid habitId);

        // Relapse
        Task<Guid> LogRelapseAsync(Guid habitId, string? reason = null, string? note = null);
        Task<IReadOnlyList<RelapseLogDto>> GetRelapsesAsync(Guid habitId);

        // Analytics / Statistics
        Task<HabitStatisticsDto> GetHabitStatisticsAsync(Guid habitId);
    }
}