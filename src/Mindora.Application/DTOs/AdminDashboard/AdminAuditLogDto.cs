namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminAuditLogDto
    {
        public long AuditId { get; set; }
        public Guid? UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string RecordId { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}