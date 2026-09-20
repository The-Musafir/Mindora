namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminUserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; }
    }
}