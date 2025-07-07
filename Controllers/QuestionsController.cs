using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CompetitionsWebsite.Controllers;

public class QuestionsController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;
    public QuestionsController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public IActionResult Quiz(int categoryId, string level)
    {
        var category = _context.Categories.Find(categoryId);
        if (category == null)
            return NotFound();

        var questions = _context.Questions
            .Where(q => q.CategoryId == categoryId && q.Level == level)
            .OrderBy(q => Guid.NewGuid())  // ✅ ترتيب عشوائي
            .Take(10)                      // ✅ فقط 10 أسئلة
            .ToList();

        foreach (var q in questions)
        {
            switch (q)
            {
                case MCQQuestion mcq:
                    _context.Entry(mcq).Collection(x => x.Options).Load();
                    break;

                case MatchingQuestion match:
                    _context.Entry(match).Collection(x => x.Pairs).Load();
                    break;

                case SpellingQuestion spell:
                    _context.Entry(spell).Collection(x => x.Letters).Load();
                    break;
            }
        }

        var viewModel = questions.Select(q => new QuizQuestionViewModel
        {
            Id = q.Id,
            Text = q.Text,
            Type = q.Type,
            Level = q.Level,
            CorrectWord = q.CorrectWord,
            CategoryId = categoryId,

            Options = q is MCQQuestion mcq ? mcq.Options.Select(o => o.Text).ToList() : null,
            CorrectOptionIndex = q is MCQQuestion mcq2
                ? mcq2.Options.ToList().FindIndex(o => o.IsCorrect)
                : -1,

            Items = q is MatchingQuestion match ? match.Pairs.Select(p => new Item { Text = p.LeftItem }).ToList() : null,
            Matches = q is MatchingQuestion match2 ? match2.Pairs.Select(p => new Item { Text = p.RightItem }).ToList() : null,

            Letters = q is SpellingQuestion spell ? spell.Letters.Select(l => l.Letter).ToList() : null

        }).ToList();

        ViewBag.CategoryName = category.Name;
        ViewBag.Level = level;

        return View(viewModel);
    }

    public IActionResult ChooseDifficulty(int categoryId)
    {
        var category = _context.Categories.Find(categoryId);
        if (category == null)
            return NotFound();

        ViewBag.CategoryId = categoryId;
        ViewBag.CategoryName = category.Name;

        return View();
    }

    [HttpPost]
    public IActionResult SaveResult([FromBody] QuizResultViewModel model)
    {
        var userId = _userManager.GetUserId(User); // أو حسب طريقتك الحالية

        var quizResult = new UserQuizAttempt
        {
            UserId = userId,
            Score = model.Score,
            AttemptDate = DateTime.Now,
            Answers = model.UserResponses.Select(r => new UserAnswer
            {
                QuestionId = r.QuestionId,
                QuestionType = r.QuestionType,
                UserResponse = r.UserResponse,
                IsCorrect = r.IsCorrect
            }).ToList()
        };

        // نحدد إن كانت المسابقة خاصة أو عادية
        if (model.QuizId.HasValue)
        {
            // special quiz
            quizResult.SpecialQuizAssignmentId = model.AssignmentId.Value;

            // تحديث السجل في SpecialQuizAssignment
            var assignment = _context.SpecialQuizAssignments.FirstOrDefault(a =>
                a.Id == model.AssignmentId.Value && a.UserId == userId);

            if (assignment != null)
            {
                assignment.Score = model.Score;
                assignment.CompletedAt = DateTime.Now;
                assignment.IsCompleted = true;
            }
        }
        else
        {
            // regular quiz
            quizResult.CategoryId = model.CategoryId;
            quizResult.Level = model.Level;
        }


        _context.UserQuizAttempts.Add(quizResult);
        _context.SaveChanges();
        return Ok();
    }


    public IActionResult StartSpecialQuiz(int quizId)
    {
        var userId = _userManager.GetUserId(User);
        var quiz = _context.SpecialQuizzes
            .Include(q => q.Questions)
            .ThenInclude(q => (q.Question as MCQQuestion).Options)
            .Include(q => q.Questions)
            .ThenInclude(q => (q.Question as MatchingQuestion).Pairs)
            .Include(q => q.Questions)
            .ThenInclude(q => (q.Question as SpellingQuestion).Letters)
            .FirstOrDefault(q => q.Id == quizId);

        if (quiz == null)
            return NotFound();

        var viewModel = quiz.Questions.Select(q => new QuizQuestionViewModel
        {
            Id = q.Question.Id,
            Text = q.Question.Text,
            Type = q.Question.Type,
            Level = q.Question.Level,
            CorrectWord = q.Question.CorrectWord,
            CategoryId = q.Question.CategoryId,

            Options = q.Question is MCQQuestion mcq ? mcq.Options.Select(o => o.Text).ToList() : null,
            CorrectOptionIndex = q.Question is MCQQuestion mcq2
                ? mcq2.Options.ToList().FindIndex(o => o.IsCorrect)
                : -1,

            Items = q.Question is MatchingQuestion match ? match.Pairs.Select(p => new Item { Text = p.LeftItem }).ToList() : null,
            Matches = q.Question is MatchingQuestion match2 ? match2.Pairs.Select(p => new Item { Text = p.RightItem }).ToList() : null,

            Letters = q.Question is SpellingQuestion spell ? spell.Letters.Select(l => l.Letter).ToList() : null

        }).ToList();
        var assignment = _context.SpecialQuizAssignments
        .FirstOrDefault(a => a.SpecialQuizId == quizId && a.UserId == userId);

        if (assignment == null)
            return Unauthorized();

        ViewBag.AssignmentId = assignment.Id;

        ViewBag.QuizType = "special";
        ViewBag.QuizId = quizId;


        return View("Quiz", viewModel); // نستخدم نفس الـ View quiz.cshtml
    }

}