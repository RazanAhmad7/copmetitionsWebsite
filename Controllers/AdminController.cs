using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsWebsite.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Dashboard()
    {

        var model = new DashboardViewModel
        {
            Questions = await _context.Questions.ToListAsync(),
            Categories = await _context.Categories.ToListAsync(),
            Users = await _context.Users.ToListAsync(),
        };

        return View(model);
    }
    public IActionResult ManageQuestions() => View();

    public IActionResult AddQuestion()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveQuestion([FromBody] QuestionInputViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid data");

        Question question;

        // Resolve the category ID
        int categoryId = await GetCategoryIdAsync(model.Category);
        const string defaultLevel = "Easy"; // Default level if not specified
        Category currentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id ==categoryId);
        switch (model.Type)
        {
            case "multipleChoice":
                question = new MCQQuestion
                {
                    Text = model.Text,
                    Type = "MCQ",
                    CategoryId = categoryId,
                    Level = defaultLevel,
                    Options = model.Options.Select((text, index) => new MCQOption
                    {
                        Text = text,
                        IsCorrect = index == model.CorrectAnswer
                    }).ToList(),
                    Category = currentCategory,
                };
                break;

            case "matching":
                question = new MatchingQuestion
                {
                    Text = model.Text,
                    Type = "Matching",
                    CategoryId = categoryId,
                    Level = "Easy",
                    Pairs = model.Pairs.Select(p => new MatchingPair
                    {
                        LeftItem = p.Item,
                        RightItem = p.Answer
                    }).ToList(),
                    Category = currentCategory,
                };
                break;

            case "wordBuilding":
                question = new SpellingQuestion
                {
                    Text = model.Text,
                    Type = "Spelling",
                    CategoryId = categoryId,
                    CorrectWord = model.CorrectWord,
                    Letters = model.Letters.Select(l => new SpellingLetter
                    {
                        Letter = l
                    }).ToList()
                };
                break;

            default:
                return BadRequest("Unsupported question type.");
        }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, questionId = question.Id });
    }
    // Add this method inside the class!
    private async Task<int> GetCategoryIdAsync(string categoryName)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
            throw new Exception($"Category '{categoryName}' not found");

        return category.Id;
    }

}