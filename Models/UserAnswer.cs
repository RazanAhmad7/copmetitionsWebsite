namespace CompetitionsWebsite.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }
        public int ParticipantId { get; set; }
        public int QuestionId { get; set; }
        public string AnswerText { get; set; } // Could be text, option id, or JSON for matching
        public bool IsCorrect { get; set; }
    }
}
