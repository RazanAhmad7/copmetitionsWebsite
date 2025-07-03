namespace CompetitionsWebsite.ViewModels
{
    public class QuizResultViewModel
    {
        public int CategoryId { get; set; }
        public string Level { get; set; }
        public int Score { get; set; }
       
        public List<UserAnswerViewModel> UserResponses { get; set; }
    }

    public class UserAnswerViewModel
    {
        public string? QuestionText { get; set; }
        public string? CorrectAnswer { get; set; }

        public int QuestionId { get; set; }
        public string QuestionType { get; set; }
        public string UserResponse { get; set; }
        public bool IsCorrect { get; set; }
    }

}
