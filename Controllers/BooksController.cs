using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class BooksController : Controller
    {
        private const string DefaultImagePath = "\\images\\No_Image.png";
        private const long MaxImageSize = 2 * 1024 * 1024; // 2 MB
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BooksController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult GetIndexView(string? search, string? sortType, string? sortOrder)
        {
            IQueryable<Book> books = context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher);

            if (!string.IsNullOrWhiteSpace(search))
            {
                books = books.Where(b =>
                    b.Title.Contains(search) ||
                    b.AuthorName.Contains(search) ||
                    b.ISBN.Contains(search));
            }

            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType == "Title" && sortOrder == "asc")
                    books = books.OrderBy(b => b.Title);
                else if (sortType == "Title" && sortOrder == "desc")
                    books = books.OrderByDescending(b => b.Title);
                else if (sortType == "AuthorName" && sortOrder == "asc")
                    books = books.OrderBy(b => b.AuthorName);
                else if (sortType == "AuthorName" && sortOrder == "desc")
                    books = books.OrderByDescending(b => b.AuthorName);
                else if (sortType == "Price" && sortOrder == "asc")
                    books = books.OrderBy(b => b.Price);
                else if (sortType == "Price" && sortOrder == "desc")
                    books = books.OrderByDescending(b => b.Price);
                else if (sortType == "PublicationDate" && sortOrder == "asc")
                    books = books.OrderBy(b => b.PublicationDate);
                else if (sortType == "PublicationDate" && sortOrder == "desc")
                    books = books.OrderByDescending(b => b.PublicationDate);
            }

            ViewBag.CurrentSearch = search;
            ViewBag.SortType = sortType;
            ViewBag.SortOrder = sortOrder;
            return View("Index", books.ToList());
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(GetIndexView));

        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Book? book = context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.Borrowings)
                .ThenInclude(br => br.Member)
                .FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();
            return View("Details", book);
        }

        [HttpGet]
        public IActionResult GetCreateView()
        {
            LoadSelectLists();
            return View("Create", new Book { IsAvailable = true, StockQuantity = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Book bookFromCreateView)
        {
            if (bookFromCreateView.PublicationDate > DateTime.Now)
                ModelState.AddModelError(nameof(bookFromCreateView.PublicationDate), "Publication date can't be in the future.");

            if (context.Books.Any(b => b.ISBN == bookFromCreateView.ISBN))
                ModelState.AddModelError(nameof(bookFromCreateView.ISBN), "Duplicate ISBN.");

            ValidateImage(bookFromCreateView.ImageFile);

            if (ModelState.IsValid)
            {
                SaveNewImage(bookFromCreateView);
                context.Books.Add(bookFromCreateView);
                context.SaveChanges();
                TempData["Success"] = $"\"{bookFromCreateView.Title}\" was added successfully.";
                return RedirectToAction(nameof(GetIndexView));
            }

            LoadSelectLists(bookFromCreateView.CategoryId, bookFromCreateView.PublisherId);
            return View("Create", bookFromCreateView);
        }

        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Book? book = context.Books.Find(id);
            if (book == null) return NotFound();

            LoadSelectLists(book.CategoryId, book.PublisherId);
            return View("Edit", book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrent(Book bookFromEditView)
        {
            if (!context.Books.Any(b => b.Id == bookFromEditView.Id)) return NotFound();

            if (context.Books.Any(b => b.ISBN == bookFromEditView.ISBN && b.Id != bookFromEditView.Id))
                ModelState.AddModelError(nameof(bookFromEditView.ISBN), "Duplicate ISBN.");

            if (bookFromEditView.PublicationDate > DateTime.Now)
                ModelState.AddModelError(nameof(bookFromEditView.PublicationDate), "Publication date can't be in the future.");

            ValidateImage(bookFromEditView.ImageFile);

            // Read the stored image path from the database instead of trusting the hidden form field.
            string? storedImagePath = context.Books.AsNoTracking()
                .Where(b => b.Id == bookFromEditView.Id)
                .Select(b => b.ImagePath)
                .FirstOrDefault();
            bookFromEditView.ImagePath = storedImagePath ?? DefaultImagePath;

            if (ModelState.IsValid)
            {
                if (bookFromEditView.ImageFile != null)
                {
                    DeletePhysicalImage(storedImagePath);
                    SaveNewImage(bookFromEditView);
                }

                context.Books.Update(bookFromEditView);
                context.SaveChanges();
                TempData["Success"] = $"\"{bookFromEditView.Title}\" was updated successfully.";
                return RedirectToAction(nameof(GetIndexView));
            }

            LoadSelectLists(bookFromEditView.CategoryId, bookFromEditView.PublisherId);
            return View("Edit", bookFromEditView);
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Book? book = context.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.Borrowings)
                .FirstOrDefault(b => b.Id == id);

            if (book == null) return NotFound();
            return View("Delete", book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCurrent(int id)
        {
            Book? book = context.Books.Include(b => b.Borrowings).FirstOrDefault(b => b.Id == id);
            if (book == null) return NotFound();

            if (book.Borrowings.Any())
            {
                ModelState.AddModelError(string.Empty, "You can't delete a book that has borrowing history.");
                book = context.Books.Include(b => b.Category).Include(b => b.Publisher).Include(b => b.Borrowings).First(b => b.Id == id);
                return View("Delete", book);
            }

            DeletePhysicalImage(book.ImagePath);
            context.Books.Remove(book);
            context.SaveChanges();
            TempData["Success"] = $"\"{book.Title}\" was deleted.";
            return RedirectToAction(nameof(GetIndexView));
        }

        private void LoadSelectLists(int? categoryId = null, int? publisherId = null)
        {
            ViewBag.AllCategories = new SelectList(context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToList(), "Id", "Name", categoryId);
            ViewBag.AllPublishers = new SelectList(context.Publishers.OrderBy(p => p.Name).ToList(), "Id", "Name", publisherId);
        }

        // Only small image files with a known extension are accepted as book covers.
        private void ValidateImage(IFormFile? imageFile)
        {
            if (imageFile == null) return;

            string extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(extension))
                ModelState.AddModelError(nameof(Book.ImageFile), "Only .jpg, .jpeg, .png and .webp images are allowed.");

            if (imageFile.Length > MaxImageSize)
                ModelState.AddModelError(nameof(Book.ImageFile), "Image size mustn't exceed 2 MB.");
        }

        private void SaveNewImage(Book book)
        {
            if (book.ImageFile == null)
            {
                book.ImagePath = DefaultImagePath;
                return;
            }

            Directory.CreateDirectory(Path.Combine(webHostEnvironment.WebRootPath, "images"));
            Guid imageGuid = Guid.NewGuid();
            string extension = Path.GetExtension(book.ImageFile.FileName).ToLowerInvariant();
            string imageNewName = imageGuid + extension;
            book.ImagePath = "\\images\\" + imageNewName;
            string imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "images", imageNewName);

            using FileStream stream = new(imageFullPath, FileMode.Create);
            book.ImageFile.CopyTo(stream);
        }

        private void DeletePhysicalImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath) || imagePath == DefaultImagePath)
                return;

            string fileName = Path.GetFileName(imagePath.Replace('\\', '/'));
            if (fileName == "No_Image.png") return;

            string fullPath = Path.Combine(webHostEnvironment.WebRootPath, "images", fileName);
            if (System.IO.File.Exists(fullPath))
                System.IO.File.Delete(fullPath);
        }
    }
}
