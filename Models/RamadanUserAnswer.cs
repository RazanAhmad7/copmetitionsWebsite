namespace CompetitionsWebsite.Models
{
    public class RamadanUserAnswer
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int QuestionId { get; set; }  // من جدول RamadanCompetitionQuestion
        public string Answer { get; set; }
        public DateTime AnsweredAt { get; set; }

        public RamadanCompetitionQuestion Question { get; set; }
    }
}
