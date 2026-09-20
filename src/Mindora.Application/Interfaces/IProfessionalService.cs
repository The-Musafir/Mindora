using Mindora.Application.DTOs.Professional;

namespace Mindora.Application.Interfaces
{
    public interface IProfessionalService
    {
        // Provider Directory
        Task<IReadOnlyList<ProviderDto>> GetAllProvidersAsync(string? searchTerm = null, Guid? specialtyId = null);
        Task<ProviderDto?> GetProviderByIdAsync(Guid providerId);

        // Provider Management
        Task<Guid> CreateProviderAsync(CreateProviderRequest request);
        Task<ProviderDto> UpdateProviderAsync(UpdateProviderRequest request);
        Task<bool> ToggleProviderActiveAsync(Guid providerId, bool isActive);

        // Specialty Management
        Task<IReadOnlyList<SpecialtyDto>> GetAllSpecialtiesAsync();
        Task<Guid> CreateSpecialtyAsync(SpecialtyDto dto);

        // Practice / Service / Availability
        Task<Guid> CreatePracticeAsync(CreatePracticeRequest request);
        Task<Guid> AddServiceAsync(CreateServiceRequest request);
        Task<Guid> AddAvailabilitySlotAsync(CreateAvailabilitySlotRequest request);

        // Appointments
        Task<AppointmentDto> BookAppointmentAsync(Guid userId, CreateAppointmentRequest request);
        Task<IReadOnlyList<AppointmentDto>> GetUserAppointmentsAsync(Guid userId);
        Task<IReadOnlyList<AppointmentDto>> GetProviderAppointmentsAsync(Guid providerId);
        Task<bool> UpdateAppointmentStatusAsync(Guid appointmentId, string newStatus);
        Task<bool> CancelAppointmentAsync(Guid appointmentId, Guid userId);

        // Reviews
        Task<Guid> AddReviewAsync(Guid userId, CreateReviewRequest request);
        Task<IReadOnlyList<ReviewDto>> GetProviderReviewsAsync(Guid providerId);
        Task<double?> GetProviderAverageRatingAsync(Guid providerId);

        // Verification
        Task<Guid> UploadVerificationDocumentAsync(Guid providerId, ProviderVerificationDocumentDto dto);
        Task<IReadOnlyList<ProviderVerificationDocumentDto>> GetVerificationDocumentsAsync(Guid providerId);
        Task<bool> UpdateVerificationDocumentStatusAsync(Guid documentId, string status);
    }
}