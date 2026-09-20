using System;

namespace Mindora.Domain.Entities
{
    public class UserProfile
    {
        public Guid ProfileId { get; set; }
        public Guid UserId { get; set; }

        
        public string DisplayName { get; set; } = string.Empty;

      
        public string? Bio { get; set; }

        public DateTime? DateOfBirth { get; set; }

       
        public string? Gender { get; set; }

      
        public string? AvatarUrl { get; set; }

        public string Timezone { get; set; } = "UTC";

       
        public DateTime? ConsentGivenAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        
        public virtual User User { get; set; } = null!;
    }
}