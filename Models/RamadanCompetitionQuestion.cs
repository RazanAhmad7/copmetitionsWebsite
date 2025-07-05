namespace CompetitionsWebsite.Models
{
    public class RamadanCompetitionQuestion
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public DateTime ShowFrom { get; set; }
        public DateTime ShowTo { get; set; }
    }
}
