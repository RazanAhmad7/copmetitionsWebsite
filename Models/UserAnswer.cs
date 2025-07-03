namespace CompetitionsWebsite.Models
{
    public class UserAnswer
    {
        public int Id { get; set; }

        public int UserQuizAttemptId { get; set; }
        public UserQuizAttempt UserQuizAttempt { get; set; }

        public int QuestionId { get; set; } // رقم السؤال الذي أجاب عليه
        public Question Question { get; set; }

        public string QuestionType { get; set; } // "mcq", "matching", "spelling"

        public string UserResponse { get; set; } // للإجابة النصية أو الخيار المختار أو JSON لتطابق
        public bool IsCorrect { get; set; }
    }

}
