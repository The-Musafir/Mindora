namespace Mindora.Application.DTOs.Community
{
    public class UpdatePostRequest
    {
        public Guid PostId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public Guid? GroupId { get; set; }
        public string? Category { get; set; }
        public bool IsAnonymous { get; set; }
        public bool IsPinned { get; set; }
    }
}