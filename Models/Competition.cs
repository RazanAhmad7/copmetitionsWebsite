namespace CompetitionsWebsite.Models
{
    public class Competition
    {
        public int Id { get; set; }

        // Score for this quiz session
        public int Score { get; set; }

        // Reference to the user taking the quiz
        public string UserId { get; set; }
        public User User { get; set; }

        // Questions included in this quiz session
        public ICollection<Question> Questions { get; set; }

       
    }
}
