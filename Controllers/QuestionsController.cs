using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsWebsite.Controllers;

public class QuestionsController : Controller
{
    private readonly AppDbContext _context;
    public QuestionsController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Quiz(int categoryId, string level)
    {
        var category = _context.Categories.Find(categoryId);
        if (category == null)
            return NotFound();

        var questions = _context.Questions
            .Where(q => q.CategoryId == categoryId && q.Level == level)
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

            // Multiple Choice
            Options = q is MCQQuestion mcq ? mcq.Options.Select(o => o.Text).ToList() : null,
            CorrectOptionIndex = q is MCQQuestion mcq2
            ? mcq2.Options.ToList().FindIndex(o => o.IsCorrect)
            : -1,


            // Matching
            Items = q is MatchingQuestion match ? match.Pairs.Select(p => new Item { Text = p.LeftItem }).ToList() : null,
            Matches = q is MatchingQuestion match2 ? match2.Pairs.Select(p => new Item { Text = p.RightItem }).ToList() : null,

            // Spelling
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
}