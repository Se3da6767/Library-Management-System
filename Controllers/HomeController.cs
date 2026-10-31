using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LibrarySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext context;

        public HomeController(AppDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            DateTime today = DateTime.Today;

            ViewBag.LibraryName = "Knowledge Hub Library";
            ViewBag.OpeningHours = "09:00 AM - 08:00 PM";
            ViewBag.BooksCount = context.Books.Count();
            ViewBag.AvailableBooks = context.Books.Count(b => b.IsAvailable);
            ViewBag.CategoriesCount = context.Categories.Count();
            ViewBag.PublishersCount = context.Publishers.Count();
            ViewBag.MembersCount = context.Members.Count(m => m.IsActive);
            ViewBag.ActiveBorrowings = context.Borrowings.Count(b => !b.IsReturned);
            ViewBag.OverdueBorrowings = context.Borrowings.Count(b => !b.IsReturned && b.DueDate < today);

            ViewBag.RecentBorrowings = context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .OrderByDescending(b => b.BorrowDate)
                .ThenByDescending(b => b.Id)
                .Take(5)
                .ToList();

            ViewBag.LatestBooks = context.Books
                .OrderByDescending(b => b.Id)
                .Take(4)
                .ToList();

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
