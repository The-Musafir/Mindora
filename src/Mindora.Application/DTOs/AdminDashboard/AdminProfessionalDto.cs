namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminProfessionalDto
    {
        public Guid ProviderId { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public bool IsVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Specialties { get; set; } = new();
    }
}