namespace CompetitionsWebsite.Models
{
    public class SpecialQuiz
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Title { get; set; } // مثال: "مسابقة خاصة للمجموعة أ"

        public ICollection<SpecialQuizQuestion> Questions { get; set; }
        public ICollection<SpecialQuizAssignment> Assignments { get; set; }
    }
}
