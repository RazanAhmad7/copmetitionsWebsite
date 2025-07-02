using CompetitionsWebsite.Models;
using CompetitionsWebsite.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CompetitionsWebsite.Controllers
{
    public class CategoryController : Controller
    {

        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult AddCategory()
        {
            return View();
        }


        // POST: Admin/SaveCategory
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult SaveCategory([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return View("AddCategory", model);

            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.Categories.Add(category);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "تم حفظ القسم بنجاح!";

            return Ok(new { success = true });
        }

        public IActionResult EditCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return NotFound();
            var model = new CategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCategory([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            var category = _context.Categories.FirstOrDefault(c => c.Id == model.Id);
            if (category == null)
                return NotFound();

            category.Name = model.Name;
            category.Description = model.Description;
            _context.SaveChanges();

            return Ok(new { success = true });
        }

        [HttpGet]
        public IActionResult DeleteCategory(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["DeleteMessage"] = "لم يتم العثور على القسم.";
                return RedirectToAction("Dashboard", "Admin", new { tab = "categories" });
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["DeleteMessage"] = "تم حذف القسم وجميع أسئلته بنجاح!";
            return RedirectToAction("Dashboard", "Admin", new { tab = "categories" });
        }
    }
}
