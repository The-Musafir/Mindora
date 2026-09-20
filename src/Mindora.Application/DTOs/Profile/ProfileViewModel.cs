namespace Mindora.Application.DTOs.Profile
{
   
    public class ProfileViewModel
    {
        // ============================================================
        // Identity (User)
        // ============================================================
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();

        // ============================================================
        // Profile
        // ============================================================
        public Guid? ProfileId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string Timezone { get; set; } = "UTC";
        public DateTime? ConsentGivenAt { get; set; }

        // ============================================================
        // Computed
        // ============================================================
        public string PrimaryRole =>
            Roles.Contains("Admin") ? "Admin" :
            Roles.Contains("Professional") ? "Professional" :
            Roles.Contains("Moderator") ? "Moderator" :
            "Member";

        public string RoleBadgeColor =>
            PrimaryRole switch
            {
                "Admin" => "#EF4444",
                "Professional" => "#8B5CF6",
                "Moderator" => "#F59E0B",
                _ => "#06B6D4"
            };

        public string Initial =>
            !string.IsNullOrEmpty(DisplayName)
                ? DisplayName.Substring(0, 1).ToUpper()
                : "U";

        public int BioWordCount =>
            string.IsNullOrWhiteSpace(Bio)
                ? 0
                : Bio.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}