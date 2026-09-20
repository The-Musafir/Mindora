namespace Mindora.Application.DTOs.Professional
{
    public class ProviderVerificationDocumentDto
    {
        public Guid DocumentId { get; set; }
        public Guid ProviderId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime? VerifiedAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}