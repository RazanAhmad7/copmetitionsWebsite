namespace CompetitionsWebsite.Models
{
    public class SpecialQuizQuestion
    {
        public int Id { get; set; }

        public int SpecialQuizId { get; set; }
        public SpecialQuiz SpecialQuiz { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
