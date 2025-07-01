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

    public async Task<IActionResult> AddQuestion()
    {
        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

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
        Category currentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
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
                    Level = "Easy",
                    CorrectWord = model.CorrectWord,
                    Letters = model.Letters.Select(l => new SpellingLetter
                    {
                        Letter = l
                    }).ToList(),
                    Category = currentCategory,
                };
                break;

            default:
                return BadRequest("Unsupported question type.");
        }

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, questionId = question.Id });
    }

    [HttpGet]
    public async Task<IActionResult> EditQuestion(int id)
    {
        var question = await _context.Questions
            .Include(q => q.Category)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return NotFound("Question not found");

        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

        // Load specific question data based on type
        QuestionInputViewModel model = new QuestionInputViewModel
        {
            Id = question.Id.ToString(),
            Type = question.Type.ToLower() switch
            {
                "mcq" => "multipleChoice",
                "matching" => "matching",
                "spelling" => "wordBuilding",
                _ => question.Type.ToLower()
            },
            Text = question.Text,
            Category = question.Category?.Name ?? ""
        };

        switch (question.Type)
        {
            case "MCQ":
                var mcqQuestion = await _context.MCQQuestions
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (mcqQuestion != null)
                {
                    model.Options = mcqQuestion.Options.Select(o => o.Text).ToList();
                    model.CorrectAnswer = mcqQuestion.Options.ToList().FindIndex(o => o.IsCorrect);
                }
                break;

            case "Matching":
                var matchingQuestion = await _context.MatchingQuestions
                    .Include(q => q.Pairs)
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (matchingQuestion != null)
                {
                    model.Pairs = matchingQuestion.Pairs.Select(p => new MatchingPairViewModel
                    {
                        Item = p.LeftItem,
                        Answer = p.RightItem
                    }).ToList();
                }
                break;

            case "Spelling":
                var spellingQuestion = await _context.SpellingQuestions
                    .Include(q => q.Letters)
                    .FirstOrDefaultAsync(q => q.Id == id);
                if (spellingQuestion != null)
                {
                    model.Letters = spellingQuestion.Letters.Select(l => l.Letter).ToList();
                    model.CorrectWord = spellingQuestion.CorrectWord;
                }
                break;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQuestion([FromBody] QuestionInputViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid data");

        var existingQuestion = await _context.Questions.FindAsync(int.Parse(model.Id));
        if (existingQuestion == null)
            return NotFound("Question not found");

        // Resolve the category ID
        int categoryId = await GetCategoryIdAsync(model.Category);
        Category currentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

        // Update basic question properties
        existingQuestion.Text = model.Text;
        existingQuestion.CategoryId = categoryId;
        existingQuestion.Category = currentCategory;

        // Remove existing related data
        switch (existingQuestion.Type)
        {
            case "MCQ":
                var mcqQuestion = await _context.MCQQuestions
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.Id == existingQuestion.Id);
                if (mcqQuestion != null)
                {
                    _context.MCQOptions.RemoveRange(mcqQuestion.Options);
                }
                break;

            case "Matching":
                var matchingQuestion = await _context.MatchingQuestions
                    .Include(q => q.Pairs)
                    .FirstOrDefaultAsync(q => q.Id == existingQuestion.Id);
                if (matchingQuestion != null)
                {
                    _context.MatchingPairs.RemoveRange(matchingQuestion.Pairs);
                }
                break;

            case "Spelling":
                var spellingQuestion = await _context.SpellingQuestions
                    .Include(q => q.Letters)
                    .FirstOrDefaultAsync(q => q.Id == existingQuestion.Id);
                if (spellingQuestion != null)
                {
                    _context.SpellingLetters.RemoveRange(spellingQuestion.Letters);
                }
                break;
        }

        // Update with new data
        switch (model.Type)
        {
            case "multipleChoice":
                var mcqQuestion = existingQuestion as MCQQuestion;
                if (mcqQuestion != null)
                {
                    mcqQuestion.Options = model.Options.Select((text, index) => new MCQOption
                    {
                        Text = text,
                        IsCorrect = index == model.CorrectAnswer
                    }).ToList();
                }
                break;

            case "matching":
                var matchingQuestion = existingQuestion as MatchingQuestion;
                if (matchingQuestion != null)
                {
                    matchingQuestion.Pairs = model.Pairs.Select(p => new MatchingPair
                    {
                        LeftItem = p.Item,
                        RightItem = p.Answer
                    }).ToList();
                }
                break;

            case "wordBuilding":
                var spellingQuestion = existingQuestion as SpellingQuestion;
                if (spellingQuestion != null)
                {
                    spellingQuestion.CorrectWord = model.CorrectWord;
                    spellingQuestion.Letters = model.Letters.Select(l => new SpellingLetter
                    {
                        Letter = l
                    }).ToList();
                }
                break;
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true, questionId = existingQuestion.Id });
    }

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> DeleteQuestion(int id)
    //{
    //    var question = await _context.Questions.FindAsync(id);
    //    if (question == null)
    //        return NotFound("Question not found");

    //    _context.Questions.Remove(question);
    //    await _context.SaveChangesAsync();

    //    return Ok(new { success = true });
    //}

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null)
        {
            TempData["DeleteMessage"] = "لم يتم العثور على السؤال.";
            return RedirectToAction("Dashboard");
        }

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        TempData["DeleteMessage"] = "Done";
        return RedirectToAction("Dashboard");
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