using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsWebsite.Controllers;

public class ProfileController : Controller
{

    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;

        public ProfileController(UserManager<User> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public async Task<IActionResult> Profile()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

        var attempts = _context.UserQuizAttempts
            .Where(a => a.UserId == userId)
            .Include(a => a.Category)
            .Include(a => a.Answers)
            .ThenInclude(a => a.Question)
            .ToList();
        RamadanUserAnswer todayUserAnswer = null;
        bool hasAnsweredToday = false;
        var now = DateTime.Now;
        var todayRamadanQuestion = await _context.RamadanCompetitionQuestions
            .FirstOrDefaultAsync(q => q.ShowFrom <= now && q.ShowTo > now);


        var mappedAttempts = attempts.Select(a => new UserQuizAttemptViewModel
        {
            AttemptId = a.Id,
            CategoryName = a.Category?.Name ?? "غير معروف",
            Level = a.Level,
            AttemptDate = a.AttemptDate,
            Score = a.Score,
            Answers = a.Answers.Select(ans => new UserAnswerViewModel
            {
                QuestionText = ans.Question?.Text ?? "غير معروف",
                UserResponse = ans.UserResponse,
                IsCorrect = ans.IsCorrect,
                QuestionType = ans.QuestionType,
                CorrectAnswer = GetCorrectAnswer(ans.Question)
            }).ToList()
        }).ToList();

        var specialQuizzes = await _context.SpecialQuizAssignments
            .Include(usq => usq.SpecialQuiz)
                .ThenInclude(sq => sq.Questions)
            .Where(usq => usq.UserId == userId)
            .Select(usq => new SpecialQuizViewModel
            {
                QuizId = usq.SpecialQuizId,
                QuestionsCount = usq.SpecialQuiz.Questions.Count,
                IsCompleted = _context.UserQuizAttempts.Any(a => a.SpecialQuizAssignmentId == usq.Id)
            }).ToListAsync();

      
        if (todayRamadanQuestion != null)
        {
            todayUserAnswer = await _context.RamadanCompetitionAnswers
                .FirstOrDefaultAsync(a => a.UserId == userId && a.QuestionId == todayRamadanQuestion.Id);

            hasAnsweredToday = todayUserAnswer != null;
        }

        var viewModel = new ProfileViewModel
        {
            UserName = user?.UserName ?? "مستخدم",
            JoinDate = user?.JoinedAt ?? DateTime.Now,
            TotalAttempts = attempts.Count,
            AverageScore = attempts.Count > 0 ? attempts.Average(a => a.Score) : 0,
            BestScore = attempts.Count > 0 ? attempts.Max(a => a.Score) : 0,
            Attempts = mappedAttempts,
            SpecialQuizzes = specialQuizzes,
            TodayRamadanQuestion = todayRamadanQuestion,
            HasAnsweredToday = hasAnsweredToday,
            TodayRamadanUserAnswer = todayUserAnswer
        };


        return View(viewModel);
    }

    private string GetCorrectAnswer(Question question)
    {
        if (question == null)
            return "غير معروف";

        // إعادة جلب السؤال من قاعدة البيانات حسب نوعه
        var mcq = _context.MCQQuestions
            .Include(q => q.Options)
            .FirstOrDefault(q => q.Id == question.Id);

        if (mcq != null)
        {
            var correctOption = mcq.Options.FirstOrDefault(o => o.IsCorrect);
            return correctOption?.Text ?? "غير متوفرة";
        }

        var matching = _context.MatchingQuestions
            .Include(q => q.Pairs)
            .FirstOrDefault(q => q.Id == question.Id);

        if (matching != null)
        {
            return string.Join(", ", matching.Pairs.Select(p => $"{p.LeftItem} => {p.RightItem}"));
        }

        var spelling = _context.SpellingQuestions
            .FirstOrDefault(q => q.Id == question.Id);

        if (spelling != null)
        {
            return spelling.CorrectWord ?? "غير متوفرة";
        }

        return "غير معروف";
    }

}