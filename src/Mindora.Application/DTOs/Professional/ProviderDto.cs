namespace Mindora.Application.DTOs.Professional
{
    public class ProviderDto
    {
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<SpecialtyDto> Specialties { get; set; } = new();
        public List<PracticeDetailDto> Practices { get; set; } = new();
    }
}