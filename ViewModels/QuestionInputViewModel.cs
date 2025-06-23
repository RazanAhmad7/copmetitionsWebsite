namespace CompetitionsWebsite.ViewModels
{
    public class QuestionInputViewModel
    {
        public string Id { get; set; } // to match JS Date.now().toString()
        public string Type { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }

        public List<string> Options { get; set; } = new();
        public int? CorrectAnswer { get; set; }

        public List<MatchingPairViewModel> Pairs { get; set; } = new();
        public List<string> Letters { get; set; } = new();
        public string CorrectWord { get; set; }
    }

    public class MatchingPairViewModel
    {
        public string Item { get; set; }
        public string Answer { get; set; }
    }
}
