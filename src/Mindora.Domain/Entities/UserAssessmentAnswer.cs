namespace Mindora.Domain.Entities
{
    public class UserAssessmentAnswer
    {
        public Guid AnswerId { get; set; }
        public Guid UserAssessmentId { get; set; }
        public Guid QuestionId { get; set; }
        public Guid? SelectedOptionId { get; set; }
        public string? FreeText { get; set; }

        public virtual UserAssessment UserAssessment { get; set; } = null!;
        public virtual AssessmentQuestion Question { get; set; } = null!;
        public virtual AssessmentOption? SelectedOption { get; set; }
    }
}