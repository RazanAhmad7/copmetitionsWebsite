namespace CompetitionsWebsite.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Type { get; set; } // e.g., "MCQ", "True/False", etc.
        public string Level { get; set; } // e.g., "Easy", "Medium", "Hard"

        public string? CorrectWord { get; set; }
        public int CategoryId { get; set; }
        // Navigation property for the competition
        public Category Category { get; set; }
   

    }
}
