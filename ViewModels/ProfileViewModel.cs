using CompetitionsWebsite.Models;

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
        public List<SpecialQuizViewModel> SpecialQuizzes { get; set; }

        public RamadanCompetitionQuestion? TodayRamadanQuestion { get; set; }
        public bool? HasAnsweredToday { get; set; }
        public RamadanUserAnswer? TodayRamadanUserAnswer { get; set; }



    }

    public class SpecialQuizViewModel
    {
        public int QuizId { get; set; }
        public string CategoryName { get; set; }
        public DateTime AssignedDate { get; set; }
        public int QuestionsCount { get; set; }
    }

}
