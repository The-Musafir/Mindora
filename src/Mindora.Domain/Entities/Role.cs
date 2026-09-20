using Microsoft.AspNetCore.Identity;

namespace Mindora.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public bool IsDeleted { get; set; } = false;
    }
}