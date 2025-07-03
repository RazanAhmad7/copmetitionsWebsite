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
                CorrectAnswer = GetCorrectAnswer(ans.Question)
            }).ToList()
        }).ToList();

        var viewModel = new ProfileViewModel
        {
            UserName = user?.UserName ?? "مستخدم",
            JoinDate = user?.JoinedAt ?? DateTime.Now,
            TotalAttempts = attempts.Count,
            AverageScore = attempts.Count > 0 ? (int)attempts.Average(a => a.Score) : 0,
            BestScore = attempts.Count > 0 ? attempts.Max(a => a.Score) : 0,
            Attempts = mappedAttempts
        };

        return View(viewModel);
    }

    private string GetCorrectAnswer(Question question)
    {
        switch (question)
        {
            case MCQQuestion mcq when mcq.Options != null:
                var correctOption = mcq.Options.FirstOrDefault(o => o.IsCorrect);
                return correctOption?.Text ?? "غير متوفرة";

            case MatchingQuestion match when match.Pairs != null:
                return string.Join(", ", match.Pairs.Select(p => $"{p.LeftItem} => {p.RightItem}"));

            case SpellingQuestion spell:
                return spell.CorrectWord ?? "غير متوفرة";

            default:
                return "غير معروف";
        }
    }


}