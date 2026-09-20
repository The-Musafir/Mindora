namespace Mindora.Domain.Entities
{
    public class AssessmentQuestion
    {
        public Guid QuestionId { get; set; }
        public Guid QuestionnaireId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public virtual AssessmentQuestionnaire Questionnaire { get; set; } = null!;
        public virtual ICollection<AssessmentOption> Options { get; set; } = new List<AssessmentOption>();
    }
}