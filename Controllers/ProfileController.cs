using Microsoft.AspNetCore.Mvc;

namespace CompetitionsWebsite.Controllers;

public class ProfileController : Controller
{
    public IActionResult Profile() => View();
}