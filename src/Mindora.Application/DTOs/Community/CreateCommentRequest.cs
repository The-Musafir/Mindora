namespace Mindora.Application.DTOs.Community
{
    public class CreateCommentRequest
    {
        public Guid PostId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ParentCommentId { get; set; }
    }
}