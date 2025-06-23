namespace CompetitionsWebsite.Models
{
    public class MatchingQuestion : Question
    {
        public ICollection<MatchingPair> Pairs { get; set; }

    }

    public class MatchingPair
    {
        public int Id { get; set; }
        public string LeftItem { get; set; }
        public string RightItem { get; set; }
        public int MatchingQuestionId { get; set; }
    }

}
