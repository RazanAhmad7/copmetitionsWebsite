namespace CompetitionsWebsite.ViewModels
{
    public class ProfileViewModel
    {
        public string UserName { get; set; }
        public DateTime JoinDate { get; set; }

        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public int BestScore { get; set; }

        public List<UserQuizAttemptViewModel> Attempts { get; set; }
    }

}
