namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminLoginHistoryDto
    {
        public long LoginHistoryId { get; set; }
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime LoginAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccess { get; set; }
    }
}