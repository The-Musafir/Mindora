namespace Mindora.Application.DTOs.Professional
{
    public class UpdateProviderRequest
    {
        public Guid ProviderId { get; set; }
        public string? Bio { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsActive { get; set; }
        public List<Guid> SpecialtyIds { get; set; } = new();
    }
}