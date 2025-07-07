using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsWebsite.Controllers;

public class AdminController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public AdminController(AppDbContext context , UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
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

        // تحميل جميع التعيينات المكتملة للمسابقات الخاصة مع المستخدم والمحاولات والإجابات
        var specialAssignments = await _context.SpecialQuizAssignments
            .Include(a => a.User)
            .Include(a => a.SpecialQuiz)
            .Include(a => a.Attempts)
                .ThenInclude(attempt => attempt.Answers)
                    .ThenInclude(answer => answer.Question)
            .ToListAsync();

        // تحويلها إلى ViewModel
        model.SpecialQuizAssignments = specialAssignments.Select(a => new SpecialQuizAssignmentViewModel
        {
            AssignmentId = a.Id,
            SpecialQuizId = a.SpecialQuizId, // ✅ Add this

            UserName = a.User.UserName,
            SpecialQuizTitle = a.SpecialQuiz.Title,
            CompletedAt = a.CompletedAt,
            Score = a.Score,
            Attempts = a.Attempts.Select(attempt => new UserQuizAttemptViewModel
            {
                AttemptId = attempt.Id,
                CategoryName = attempt.Category?.Name ?? "بدون تصنيف",
                Level = attempt.Level ?? "",
                AttemptDate = attempt.AttemptDate,
                Score = attempt.Score,
                Answers = attempt.Answers.Select(ans => new UserAnswerViewModel
                {
                    QuestionId = ans.QuestionId,
                    QuestionText = ans.Question?.Text ?? "",
                    CorrectAnswer = ans.Question?.CorrectWord ?? "",
                    QuestionType = ans.QuestionType,
                    UserResponse = ans.UserResponse,
                    IsCorrect = ans.IsCorrect
                }).ToList()
            }).ToList()
        }).ToList();

        return View(model);
    }

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
      
        Category currentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
        switch (model.Type)
        {
            case "multipleChoice":
                question = new MCQQuestion
                {
                    Text = model.Text,
                    Type = "MCQ",
                    CategoryId = categoryId,
                    Level = model.Level,
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
                    Level = model.Level,
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
                    Level = model.Level,
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
            Category = question.Category?.Name ?? "",
            Level = question.Level,
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
        existingQuestion.Level = model.Level;

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


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSpecialQuiz([FromBody] SpecialQuizDto dto)
    {
        if (dto == null || dto.QuestionIds == null || dto.UserIds == null)
            return BadRequest("Invalid data");

        var quiz = new SpecialQuiz
        {
            Title = "مسابقة خاصة", // يمكنك تعديل العنوان أو جعله قابلاً للإدخال لاحقًا
            CreatedAt = DateTime.Now,
            Questions = dto.QuestionIds.Select(qid => new SpecialQuizQuestion
            {
                QuestionId = qid
            }).ToList(),
            Assignments = dto.UserIds.Select(uid => new SpecialQuizAssignment
            {
                UserId = uid,
                IsCompleted = false
            }).ToList()
        };

        _context.SpecialQuizzes.Add(quiz);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    // عرض النموذج
    [HttpGet]
    public IActionResult CreateRamadanQuestions()
    {
        int? questionNumber = HttpContext.Session.GetInt32("RamadanQuestionNumber");

        if (questionNumber == null)
        {
            // الجلسة منتهية؟ نحسب كم سؤال موجود في قاعدة البيانات
            int existingQuestionsCount = _context.RamadanCompetitionQuestions.Count();
            questionNumber = existingQuestionsCount + 1;

            // نعيد تخزينها في الجلسة
            HttpContext.Session.SetInt32("RamadanQuestionNumber", questionNumber.Value);
        }

        // إذا انتهينا من 30 سؤال، لا نسمح بالمزيد
        if (questionNumber > 30)
        {
            return RedirectToAction("Dashboard", "Admin"); // أو عرض رسالة خاصة
        }

        var model = new RamadanQuestionInputViewModel
        {
            QuestionNumber = questionNumber.Value
        };

        return View(model);
    }

    // حفظ السؤال
    [HttpPost]
    public IActionResult CreateRamadanQuestions(RamadanQuestionInputViewModel model)
    {
        int questionNumber = HttpContext.Session.GetInt32("RamadanQuestionNumber") ?? 1;

        // أول مرة؟ نحفظ التاريخ الأساسي
        DateTime startDate;
        if (questionNumber == 1)
        {
            startDate = model.StartDate.Value.Date;
            HttpContext.Session.SetString("RamadanStartDate", startDate.ToString("yyyy-MM-dd"));
        }
        else
        {
            startDate = DateTime.Parse(HttpContext.Session.GetString("RamadanStartDate"));
        }

        DateTime showFrom = startDate.AddDays(questionNumber - 1).AddHours(21); // 9 PM
        DateTime showTo = startDate.AddDays(questionNumber).AddHours(5);       // 5 AM next day

        var question = new RamadanCompetitionQuestion
        {
            QuestionText = model.QuestionText,
            ShowFrom = showFrom,
            ShowTo = showTo
        };

        _context.RamadanCompetitionQuestions.Add(question);
        _context.SaveChanges();

        // Update Session
        HttpContext.Session.SetInt32("RamadanQuestionNumber", questionNumber + 1);

        if (questionNumber == 30)
            return RedirectToAction("Dashboard");

        return RedirectToAction("CreateRamadanQuestions");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitRamadanAnswer(int QuestionId, string Answer)
    {
        var userId = _userManager.GetUserId(User);

        // التأكد أن المستخدم لم يجب على هذا السؤال من قبل
        var alreadyAnswered = await _context.RamadanCompetitionAnswers
            .AnyAsync(x => x.UserId == userId && x.QuestionId == QuestionId);

        if (alreadyAnswered)
        {
            TempData["Message"] = "لقد أجبت على هذا السؤال بالفعل.";
            return RedirectToAction("Profile","Profile");
        }

        var newAnswer = new RamadanUserAnswer
        {
            UserId = userId,
            QuestionId = QuestionId,
            Answer = Answer,
            AnsweredAt = DateTime.Now
        };

        _context.RamadanCompetitionAnswers.Add(newAnswer);
        await _context.SaveChangesAsync();

        TempData["Message"] = "تم إرسال إجابتك بنجاح، شكراً لمشاركتك!";
        return RedirectToAction("Profile","Profile");
    }


}