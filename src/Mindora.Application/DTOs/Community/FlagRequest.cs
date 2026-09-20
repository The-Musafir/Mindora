namespace Mindora.Application.DTOs.Community
{
    public class FlagRequest
    {
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}