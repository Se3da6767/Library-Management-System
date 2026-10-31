using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly AppDbContext context;

        public CategoriesController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetIndexView() => View("Index", context.Categories.Include(c => c.Books).ToList());

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(GetIndexView));

        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Category? category = context.Categories
                .Include(c => c.Books)
                .FirstOrDefault(c => c.Id == id);

            if (category == null) return NotFound();

            ViewBag.PageNote = "Books available in this category";
            return View("Details", category);
        }

        [HttpGet]
        public IActionResult GetCreateView() => View("Create", new Category { IsActive = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Category categoryFromCreateView)
        {
            if (categoryFromCreateView.Name == categoryFromCreateView.Description)
                ModelState.AddModelError(string.Empty, "Description must be different from the name.");

            if (context.Categories.Any(c => c.Name == categoryFromCreateView.Name))
                ModelState.AddModelError(nameof(categoryFromCreateView.Name), "Duplicate category name.");

            if (ModelState.IsValid)
            {
                context.Categories.Add(categoryFromCreateView);
                context.SaveChanges();
                TempData["Success"] = $"Category \"{categoryFromCreateView.Name}\" was added.";
                return RedirectToAction(nameof(GetIndexView));
            }

            return View("Create", categoryFromCreateView);
        }

        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Category? category = context.Categories.Find(id);
            if (category == null) return NotFound();
            return View("Edit", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrent(Category categoryFromEditView)
        {
            if (context.Categories.Any(c => c.Name == categoryFromEditView.Name && c.Id != categoryFromEditView.Id))
                ModelState.AddModelError(nameof(categoryFromEditView.Name), "Duplicate category name.");

            if (categoryFromEditView.Name == categoryFromEditView.Description)
                ModelState.AddModelError(string.Empty, "Description must be different from the name.");

            if (ModelState.IsValid)
            {
                context.Categories.Update(categoryFromEditView);
                context.SaveChanges();
                TempData["Success"] = $"Category \"{categoryFromEditView.Name}\" was updated.";
                return RedirectToAction(nameof(GetIndexView));
            }

            return View("Edit", categoryFromEditView);
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Category? category = context.Categories.Include(c => c.Books).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();
            return View("Delete", category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCurrent(int id)
        {
            Category? category = context.Categories.Include(c => c.Books).FirstOrDefault(c => c.Id == id);
            if (category == null) return NotFound();

            if (category.Books.Any())
            {
                ModelState.AddModelError(string.Empty, "You can't delete a category that contains books.");
                return View("Delete", category);
            }

            context.Categories.Remove(category);
            context.SaveChanges();
            TempData["Success"] = $"Category \"{category.Name}\" was deleted.";
            return RedirectToAction(nameof(GetIndexView));
        }
    }
}
