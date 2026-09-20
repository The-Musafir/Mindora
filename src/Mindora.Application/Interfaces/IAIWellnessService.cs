using Mindora.Application.DTOs.AI;

namespace Mindora.Application.Interfaces
{
    public interface IAIWellnessService
    {
        // ============================
        // SESSION MANAGEMENT
        // ============================
        Task<AIWellnessCoachSessionDto> CreateSessionAsync(Guid userId, string? initialContext = null);
        Task<AIWellnessCoachSessionDto?> GetSessionByIdAsync(Guid sessionId);
        Task<IReadOnlyList<AIWellnessCoachSessionDto>> GetUserSessionsAsync(Guid userId);
        Task<bool> EndSessionAsync(Guid sessionId);

        /// <summary>
        /// Deletes an AI session and all its messages.
        /// Only the owner can delete their own session.
        /// Returns true if deleted, false if not found or unauthorized.
        /// </summary>
        Task<bool> DeleteSessionAsync(Guid sessionId, Guid userId);

        /// <summary>
        /// Sets a custom title for an AI session.
        /// Pass null/empty to clear and revert to auto-derived title.
        /// </summary>
        Task<bool> RenameSessionAsync(Guid sessionId, Guid userId, string? newTitle);

        // ============================
        // MESSAGE / CHAT
        // ============================
        Task<AIChatMessageDto> SendMessageAsync(Guid userId, SendMessageRequest request);
        Task<IReadOnlyList<AIChatMessageDto>> GetSessionMessagesAsync(Guid sessionId);

        // ============================
        // PROMPT TEMPLATES
        // ============================
        Task<IReadOnlyList<AIPromptTemplateDto>> GetAllPromptTemplatesAsync();
        Task<Guid> CreatePromptTemplateAsync(AIPromptTemplateDto dto);
        Task<bool> TogglePromptTemplateAsync(Guid templateId, bool isActive);

        // ============================
        // FEEDBACK
        // ============================
        Task<Guid> AddFeedbackAsync(Guid userId, CreateAIFeedbackRequest request);
        Task<IReadOnlyList<AIFeedbackDto>> GetSessionFeedbacksAsync(Guid sessionId);

        // ============================
        // INTERACTION LOGS
        // ============================
        Task<IReadOnlyList<AIInteractionLogDto>> GetSessionLogsAsync(Guid sessionId);

        // ============================
        // WELLNESS SCORE
        // ============================
        Task<WellnessScoreDto> CalculateAndSaveWellnessScoreAsync(CalculateWellnessScoreRequest request);
        Task<IReadOnlyList<WellnessScoreDto>> GetUserWellnessScoresAsync(Guid userId);

        // ============================
        // RISK PREDICTION
        // ============================
        Task<RiskPredictionDto> CreateRiskPredictionAsync(CreateRiskPredictionRequest request);
        Task<IReadOnlyList<RiskPredictionDto>> GetUserRiskPredictionsAsync(Guid userId);
    }
}