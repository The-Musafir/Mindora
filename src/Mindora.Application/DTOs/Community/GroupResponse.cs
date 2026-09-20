namespace Mindora.Application.DTOs.Community
{
    public class GroupResponse
    {
        public Guid GroupId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public int PostCount { get; set; }
    }
}