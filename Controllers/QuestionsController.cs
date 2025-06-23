using Microsoft.AspNetCore.Mvc;

namespace CompetitionsWebsite.Controllers;

public class QuestionsController : Controller
{
    public IActionResult Quiz() => View();
    public IActionResult ChooseDifficulty() => View();
}