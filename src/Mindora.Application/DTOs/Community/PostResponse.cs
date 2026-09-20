namespace Mindora.Application.DTOs.Community
{
    public class PostResponse
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public bool IsAnonymous { get; set; }
        public Guid? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? Category { get; set; }
        public bool IsPinned { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }
}