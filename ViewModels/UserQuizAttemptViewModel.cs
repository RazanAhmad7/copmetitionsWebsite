namespace CompetitionsWebsite.ViewModels
{
    public class UserQuizAttemptViewModel
    {
        public int AttemptId { get; set; }
        public string CategoryName { get; set; }
        public string Level { get; set; }
        public DateTime AttemptDate { get; set; }
        public int Score { get; set; }
        public List<UserAnswerViewModel> Answers { get; set; } // للتفاصيل عند الضغط
    }

}
