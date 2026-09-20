namespace Mindora.Application.DTOs.Professional
{
    public class CreateProviderRequest
    {
        public Guid UserId { get; set; }
        public string? Bio { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public List<Guid> SpecialtyIds { get; set; } = new();
    }
}