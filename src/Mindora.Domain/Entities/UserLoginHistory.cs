using System;

namespace Mindora.Domain.Entities
{
    public class UserLoginHistory
    {
        public long LoginHistoryId { get; set; }
        public Guid UserId { get; set; }
        public DateTime LoginAt { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccess { get; set; } = true;
    }
}