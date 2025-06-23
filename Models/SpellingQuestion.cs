namespace CompetitionsWebsite.Models
{
    public class SpellingQuestion : Question
    {
        public string CorrectWord { get; set; }
        public ICollection<SpellingLetter> Letters { get; set; }
    }

    public class SpellingLetter
    {
        public int Id { get; set; }
        public string Letter { get; set; }
        public int SpellingQuestionId { get; set; }
    }
}
