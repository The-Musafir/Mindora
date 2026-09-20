namespace Mindora.Application.DTOs.Follow
{
    public class FollowUserDto
    {
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public string Role { get; set; } = "Member";
        public DateTime FollowedAt { get; set; }

        public string Initial =>
            !string.IsNullOrEmpty(DisplayName)
                ? DisplayName.Substring(0, 1).ToUpper()
                : "U";
    }
}