namespace Mindora.Domain.Entities
{
    public class UserAssessment
    {
        public Guid UserAssessmentId { get; set; }
        public Guid UserId { get; set; }
        public Guid QuestionnaireId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = "InProgress"; // InProgress, Completed

        public virtual User User { get; set; } = null!;
        public virtual AssessmentQuestionnaire Questionnaire { get; set; } = null!;
        public virtual ICollection<UserAssessmentAnswer> Answers { get; set; } = new List<UserAssessmentAnswer>();
        public virtual AssessmentResult? Result { get; set; }
    }
}