namespace CompetitionsWebsite.Models
{
    public class SpecialQuizAssignment
    {
        public int Id { get; set; }

        public int SpecialQuizId { get; set; }
        public SpecialQuiz SpecialQuiz { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int? Score { get; set; } // إذا أردتِ تخزين النتيجة مباشرة هنا

        public ICollection<UserQuizAttempt> Attempts { get; set; }

    }
}
