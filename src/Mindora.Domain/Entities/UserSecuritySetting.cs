using System;

namespace Mindora.Domain.Entities
{
    public class UserSecuritySetting
    {
        public Guid SecuritySettingId { get; set; }
        public Guid UserId { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TrustedDevices { get; set; }
        public virtual User User { get; set; } = null!;
    }
}
