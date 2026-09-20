namespace Mindora.Domain.Entities
{
    public class AssessmentQuestionnaire
    {
        public Guid QuestionnaireId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<AssessmentQuestion> Questions { get; set; } = new List<AssessmentQuestion>();
        public virtual ICollection<UserAssessment> UserAssessments { get; set; } = new List<UserAssessment>();
    }
}