namespace Mindora.Application.DTOs.Community
{
    public class GroupMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }
}