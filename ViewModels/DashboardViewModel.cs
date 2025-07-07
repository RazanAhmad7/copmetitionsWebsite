using CompetitionsWebsite.Models;

namespace CompetitionsWebsite.ViewModels
{
    public class DashboardViewModel
    {
        public List<Question> Questions { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<User> Users { get; set; } = new();
        public List<RamadanCompetitionQuestion> RamadanQuestions { get; set; } = new();
        public List<SpecialQuizAssignmentViewModel> SpecialQuizAssignments { get; set; } // 👈 أضيفي هذا فقط
        public List<RamadanQuestionAnswersViewModel> RamadanQuestionAnswers { get; set; } = new();
    }

    public class RamadanQuestionAnswersViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public List<RamadanUserAnswerViewModel> UserAnswers { get; set; } = new();
    }

    public class RamadanUserAnswerViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime AnsweredAt { get; set; }
    }
}
