namespace Mindora.Application.DTOs.Consultation
{
    public class ConsultationNoteDto
    {
        public Guid NoteId { get; set; }
        public Guid SessionId { get; set; }
        public Guid ProviderId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}