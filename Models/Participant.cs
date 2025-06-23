namespace CompetitionsWebsite.Models
{
    public class Participant
    {
        public int Id { get; set; }
        public int CompetitionId { get; set; }
        public int UserId { get; set; }

        public bool IsNotified { get; set; }
        public bool HasStarted { get; set; }
        public bool HasFinished { get; set; }
        public int Score { get; set; }

        public ICollection<UserAnswer> Answers { get; set; }
    }
}
