using Mindora.Application.DTOs.Boredom;

namespace Mindora.Application.Interfaces
{
    public interface IBoredomService
    {
        
        Task<BoredomActivityDto?> GetRandomActivityAsync(Guid userId, string? category = null);

       
        Task<BoredomSessionDto> StartSessionAsync(Guid userId, Guid activityId, int? moodBefore);

       
        Task<bool> CompleteSessionAsync(Guid sessionId, Guid userId, int? moodAfter);

    
        Task<IReadOnlyList<BoredomSessionDto>> GetHistoryAsync(Guid userId, int limit = 20);

        
        Task<IReadOnlyList<BoredomCategoryDto>> GetCategoriesAsync();

    
        Task<BoredomStatsDto> GetStatsAsync(Guid userId);
    }
}