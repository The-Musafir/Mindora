using Mindora.Application.DTOs.Journal;

namespace Mindora.Application.Interfaces
{
    public interface IJournalService
    {
        Task<JournalResponse> CreateJournalAsync(Guid userId, CreateJournalRequest request);
        Task<JournalResponse?> GetJournalByIdAsync(Guid entryId);
        Task<IReadOnlyList<JournalResponse>> GetUserJournalsAsync(Guid userId, string? searchTerm = null, string? category = null);
        Task<JournalResponse> UpdateJournalAsync(UpdateJournalRequest request);
        Task<bool> DeleteJournalAsync(Guid entryId);
        Task<JournalStatisticsDto> GetJournalStatisticsAsync(Guid userId);
    }
}