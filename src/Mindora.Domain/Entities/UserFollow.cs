using System;

namespace Mindora.Domain.Entities
{
    
    public class UserFollow
    {
        public Guid FollowId { get; set; }

        
        public Guid FollowerId { get; set; }

       
        public Guid FollowingId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual User Follower { get; set; } = null!;
        public virtual User Following { get; set; } = null!;
    }
}