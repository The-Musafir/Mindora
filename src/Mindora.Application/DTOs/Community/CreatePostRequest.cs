namespace Mindora.Application.DTOs.Community
{
    public class CreatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public Guid? GroupId { get; set; }
        public string? Category { get; set; }
        public bool IsAnonymous { get; set; } = false;
    }
}