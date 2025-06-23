namespace CompetitionsWebsite.Models
{
    public class MCQQuestion : Question
    {
        public ICollection<MCQOption> Options { get; set; }
    }
    public class MCQOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int MCQQuestionId { get; set; }
    }

  
}
