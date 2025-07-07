namespace CompetitionsWebsite.ViewModels
{
    public class SpecialQuizAssignmentViewModel
    {
        public int AssignmentId { get; set; }
        public int? SpecialQuizId { get; set; } // ✅ Add this line

        public string UserName { get; set; }
        public string SpecialQuizTitle { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? Score { get; set; }

        public List<UserQuizAttemptViewModel> Attempts { get; set; }
    }
}
