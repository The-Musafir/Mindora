namespace Mindora.Application.DTOs.Community
{
    public class CommentDto
    {
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }
}