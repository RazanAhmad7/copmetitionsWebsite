namespace CompetitionsWebsite.ViewModels
{
    public class QuizQuestionViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Level { get; set; }
        public string CorrectWord { get; set; }
        public int CategoryId { get; set; }
        // Multiple Choice
        public List<string> Options { get; set; }
        public int CorrectOptionIndex { get; set; }

        // Matching
        public List<Item> Items { get; set; }
        public List<Item> Matches { get; set; }

        // Spelling
        public List<string> Letters { get; set; }
        public int? AssignmentId { get; set; } // أضف هذا


    }

    public class Item
    {
        public string Text { get; set; }
    }


}
