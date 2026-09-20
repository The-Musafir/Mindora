namespace Mindora.Application.DTOs.Community
{
    public class ModerationFlagDto
    {
        public Guid FlagId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}