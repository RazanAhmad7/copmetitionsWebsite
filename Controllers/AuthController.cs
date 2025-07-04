using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompetitionsWebsite.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _context;
    private readonly SignInManager<User> _signInManager;
    public AuthController(UserManager<User> userManager, AppDbContext context,SignInManager<User> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }


    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // التحقق من الدور
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    else if (roles.Contains("Participant"))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    // في حال كان له دور غير معروف
                    await _signInManager.SignOutAsync();
                    ModelState.AddModelError(string.Empty, "نوع المستخدم غير مصرح به.");
                    return View(model);
                }
            }

            ModelState.AddModelError(string.Empty, "معلومات التسجيل غير صحيحة");
        }

        // في حال فشل التحقق، إعادة عرض نموذج تسجيل الدخول مع الأخطاء
        return View(model);
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new User
            {
                UserName = model.FullName,
                Email = model.Email,
                JoinedAt = DateTime.UtcNow,
            };
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني مستخدم من قبل.");
                return View(model);
            }

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // أضف المستخدم إلى Role "Participant"
                await _userManager.AddToRoleAsync(user, "Participant");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

           

        }

        return View(model);
    }
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Auth");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

}