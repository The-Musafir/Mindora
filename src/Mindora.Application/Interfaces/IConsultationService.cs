using Mindora.Application.DTOs.Consultation;

namespace Mindora.Application.Interfaces
{
    public interface IConsultationService
    {
        // Consultation Session
        Task<ConsultationSessionDto> CreateSessionAsync(Guid userId, CreateConsultationSessionRequest request);
        Task<ConsultationSessionDto?> GetSessionByIdAsync(Guid sessionId);
        Task<IReadOnlyList<ConsultationSessionDto>> GetUserSessionsAsync(Guid userId);
        Task<IReadOnlyList<ConsultationSessionDto>> GetProviderSessionsAsync(Guid providerId);
        Task<ConsultationSessionDto> UpdateSessionStatusAsync(UpdateConsultationSessionStatusRequest request);

        // Consultation Note
        Task<Guid> AddNoteAsync(CreateConsultationNoteRequest request);
        Task<IReadOnlyList<ConsultationNoteDto>> GetSessionNotesAsync(Guid sessionId);

        // Consultation Prescription
        Task<Guid> AddPrescriptionAsync(CreateConsultationPrescriptionRequest request);
        Task<IReadOnlyList<ConsultationPrescriptionDto>> GetSessionPrescriptionsAsync(Guid sessionId);

        // Consultation Feedback
        Task<Guid> AddFeedbackAsync(Guid userId, CreateConsultationFeedbackRequest request);
        Task<IReadOnlyList<ConsultationFeedbackDto>> GetSessionFeedbacksAsync(Guid sessionId);

        // Follow-up Plan
        Task<Guid> AddFollowUpPlanAsync(CreateFollowUpPlanRequest request);
        Task<IReadOnlyList<FollowUpPlanDto>> GetFollowUpPlansAsync(Guid sessionId);
        Task<bool> UpdateFollowUpPlanStatusAsync(Guid followUpPlanId, bool isCompleted);

        // Consultation Reminder (Notification Integration)
        Task<Guid> AddReminderAsync(CreateConsultationReminderRequest request);
        Task<IReadOnlyList<ConsultationReminderDto>> GetSessionRemindersAsync(Guid sessionId);
        Task<bool> MarkReminderSentAsync(Guid reminderId);
    }
}