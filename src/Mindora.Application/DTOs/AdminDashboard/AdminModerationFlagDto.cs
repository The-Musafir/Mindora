namespace Mindora.Application.DTOs.AdminDashboard
{
    public class AdminModerationFlagDto
    {
        public Guid FlagId { get; set; }
        public Guid? PostId { get; set; }
        public Guid? CommentId { get; set; }
        public string ContentType { get; set; } = string.Empty; // "Post" or "Comment"
        public string ContentSnippet { get; set; } = string.Empty;
        public string ReportedByEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public Guid? ReviewedBy { get; set; }
    }
}