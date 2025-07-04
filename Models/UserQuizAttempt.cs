namespace CompetitionsWebsite.Models
{
    public class UserQuizAttempt
    {
        public int Id { get; set; }

        public string UserId { get; set; } // ربط بمحاولة مستخدم (ASP.NET Identity)
        public User User { get; set; }

        public int? CategoryId { get; set; } // ربط بالقسم (التصنيف)
        public Category Category { get; set; }

        public string? Level { get; set; }

        public DateTime AttemptDate { get; set; } = DateTime.Now;

        public int Score { get; set; } // مجموع النقاط المحققة

        public ICollection<UserAnswer> Answers { get; set; }

        // NEW: رابط بمحاولة مسابقة خاصة (إذا كانت الإجابة داخل SpecialQuiz)
        public int? SpecialQuizAssignmentId { get; set; }
        public SpecialQuizAssignment SpecialQuizAssignment { get; set; }
    }

}
