namespace CompetitionsWebsite.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }

        // The user who answered
        public string UserId { get; set; }
        public User User { get; set; }

        // The competition attempt this answer belongs to
        public int CompetitionId { get; set; }
        public Competition Competition { get; set; }

        // The question that was answered
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        // The user's answer (can be text, JSON, etc.)
        public string Answer { get; set; }

        // Whether the answer was correct
        public bool IsCorrect { get; set; }
    }

}
