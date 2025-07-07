using CompetitionsWebsite.Models;

namespace CompetitionsWebsite.ViewModels
{
    public class DashboardViewModel
    {
        public List<Question> Questions { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<User> Users { get; set; } = new();

        public List<SpecialQuizAssignmentViewModel> SpecialQuizAssignments { get; set; } // 👈 أضيفي هذا فقط


    }



}
