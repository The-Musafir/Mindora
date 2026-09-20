using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Mindora.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public virtual UserProfile? Profile { get; set; }
        public virtual UserSecuritySetting? SecuritySetting { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<DataPrivacyRequest> DataPrivacyRequests { get; set; } = new List<DataPrivacyRequest>();
    }
}