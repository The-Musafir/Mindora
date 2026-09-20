namespace Mindora.Application.DTOs.Community
{
    public class ReactionDto
    {
        public Guid ReactionId { get; set; }
        public Guid UserId { get; set; }
        public string ReactionType { get; set; } = string.Empty;
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}