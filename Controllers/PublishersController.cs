using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class PublishersController : Controller
    {
        private readonly AppDbContext context;

        public PublishersController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetIndexView() => View("Index", context.Publishers.Include(p => p.Books).ToList());

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(GetIndexView));

        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Publisher? publisher = context.Publishers.Include(p => p.Books).FirstOrDefault(p => p.Id == id);
            if (publisher == null) return NotFound();
            return View("Details", publisher);
        }

        [HttpGet]
        public IActionResult GetCreateView() => View("Create", new Publisher());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Publisher publisherFromCreateView)
        {
            if (context.Publishers.Any(p => p.Name == publisherFromCreateView.Name))
                ModelState.AddModelError(nameof(publisherFromCreateView.Name), "Duplicate publisher name.");

            if (ModelState.IsValid)
            {
                context.Publishers.Add(publisherFromCreateView);
                context.SaveChanges();
                TempData["Success"] = $"Publisher \"{publisherFromCreateView.Name}\" was added.";
                return RedirectToAction(nameof(GetIndexView));
            }
            return View("Create", publisherFromCreateView);
        }

        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Publisher? publisher = context.Publishers.Find(id);
            if (publisher == null) return NotFound();
            return View("Edit", publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrent(Publisher publisherFromEditView)
        {
            if (context.Publishers.Any(p => p.Name == publisherFromEditView.Name && p.Id != publisherFromEditView.Id))
                ModelState.AddModelError(nameof(publisherFromEditView.Name), "Duplicate publisher name.");

            if (ModelState.IsValid)
            {
                context.Publishers.Update(publisherFromEditView);
                context.SaveChanges();
                TempData["Success"] = $"Publisher \"{publisherFromEditView.Name}\" was updated.";
                return RedirectToAction(nameof(GetIndexView));
            }
            return View("Edit", publisherFromEditView);
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Publisher? publisher = context.Publishers.Include(p => p.Books).FirstOrDefault(p => p.Id == id);
            if (publisher == null) return NotFound();
            return View("Delete", publisher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCurrent(int id)
        {
            Publisher? publisher = context.Publishers.Include(p => p.Books).FirstOrDefault(p => p.Id == id);
            if (publisher == null) return NotFound();

            if (publisher.Books.Any())
            {
                ModelState.AddModelError(string.Empty, "You can't delete a publisher that has books.");
                return View("Delete", publisher);
            }

            context.Publishers.Remove(publisher);
            context.SaveChanges();
            TempData["Success"] = $"Publisher \"{publisher.Name}\" was deleted.";
            return RedirectToAction(nameof(GetIndexView));
        }
    }
}
