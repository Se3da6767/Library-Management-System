using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class BorrowingsController : Controller
    {
        // Fee charged for every day a book is returned after its due date.
        public const decimal LateFeePerDay = 5m;

        private readonly AppDbContext context;

        public BorrowingsController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetIndexView(string? status)
        {
            IQueryable<Borrowing> borrowings = context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member);

            DateTime today = DateTime.Today;
            if (status == "active")
                borrowings = borrowings.Where(b => !b.IsReturned && b.DueDate >= today);
            else if (status == "overdue")
                borrowings = borrowings.Where(b => !b.IsReturned && b.DueDate < today);
            else if (status == "returned")
                borrowings = borrowings.Where(b => b.IsReturned);

            ViewBag.CurrentStatus = status;
            return View("Index", borrowings.OrderByDescending(b => b.BorrowDate).ToList());
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(GetIndexView));

        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Borrowing? borrowing = context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.Member)
                .FirstOrDefault(b => b.Id == id);
            if (borrowing == null) return NotFound();
            return View("Details", borrowing);
        }

        [HttpGet]
        public IActionResult GetCreateView()
        {
            LoadSelectLists();
            return View("Create", new Borrowing
            {
                BorrowDate = DateTime.Today,
                DueDate = DateTime.Today.AddDays(14)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Borrowing borrowingFromCreateView)
        {
            ValidateBorrowing(borrowingFromCreateView, false);

            Book? book = context.Books.Find(borrowingFromCreateView.BookId);
            Member? member = context.Members.Find(borrowingFromCreateView.MemberId);

            if (book == null)
                ModelState.AddModelError(nameof(borrowingFromCreateView.BookId), "Selected book was not found.");
            else if (!book.IsAvailable)
                ModelState.AddModelError(nameof(borrowingFromCreateView.BookId), "Selected book is currently unavailable.");

            if (member == null || !member.IsActive)
                ModelState.AddModelError(nameof(borrowingFromCreateView.MemberId), "Selected member is unavailable or inactive.");

            if (ModelState.IsValid)
            {
                borrowingFromCreateView.IsReturned = false;
                borrowingFromCreateView.ReturnDate = null;
                borrowingFromCreateView.LateFee = 0m;
                book!.IsAvailable = false;
                context.Borrowings.Add(borrowingFromCreateView);
                context.SaveChanges();
                TempData["Success"] = $"\"{book.Title}\" was borrowed by {member!.FullName}.";
                return RedirectToAction(nameof(GetIndexView));
            }

            LoadSelectLists(borrowingFromCreateView.BookId, borrowingFromCreateView.MemberId);
            return View("Create", borrowingFromCreateView);
        }

        // One-click return: sets the return date to today, frees the book and calculates the late fee.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkReturned(int id)
        {
            Borrowing? borrowing = context.Borrowings.Include(b => b.Book).FirstOrDefault(b => b.Id == id);
            if (borrowing == null) return NotFound();

            if (!borrowing.IsReturned)
            {
                borrowing.IsReturned = true;
                borrowing.ReturnDate = DateTime.Today;
                borrowing.LateFee = CalculateLateFee(borrowing.DueDate, DateTime.Today);
                borrowing.Book.IsAvailable = true;
                context.SaveChanges();

                TempData["Success"] = borrowing.LateFee > 0
                    ? $"\"{borrowing.Book.Title}\" was returned late. Late fee: {borrowing.LateFee:0.00}."
                    : $"\"{borrowing.Book.Title}\" was returned on time.";
            }

            return RedirectToAction(nameof(GetIndexView));
        }

        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Borrowing? borrowing = context.Borrowings.Find(id);
            if (borrowing == null) return NotFound();
            LoadSelectLists(borrowing.BookId, borrowing.MemberId, includeUnavailableBookId: borrowing.BookId);
            return View("Edit", borrowing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrent(Borrowing borrowingFromEditView)
        {
            Borrowing? oldBorrowing = context.Borrowings.AsNoTracking().FirstOrDefault(b => b.Id == borrowingFromEditView.Id);
            if (oldBorrowing == null) return NotFound();

            ValidateBorrowing(borrowingFromEditView, true);

            if (ModelState.IsValid)
            {
                if (oldBorrowing.BookId != borrowingFromEditView.BookId)
                {
                    Book? oldBook = context.Books.Find(oldBorrowing.BookId);
                    Book? newBook = context.Books.Find(borrowingFromEditView.BookId);

                    if (newBook == null || (!newBook.IsAvailable && newBook.Id != oldBorrowing.BookId))
                    {
                        ModelState.AddModelError(nameof(borrowingFromEditView.BookId), "Selected book is unavailable.");
                    }
                    else
                    {
                        if (oldBook != null) oldBook.IsAvailable = true;
                        newBook.IsAvailable = borrowingFromEditView.IsReturned;
                    }
                }
                else
                {
                    Book? book = context.Books.Find(borrowingFromEditView.BookId);
                    if (book != null) book.IsAvailable = borrowingFromEditView.IsReturned;
                }

                if (borrowingFromEditView.IsReturned && borrowingFromEditView.ReturnDate == null)
                    borrowingFromEditView.ReturnDate = DateTime.Today;
                if (!borrowingFromEditView.IsReturned)
                    borrowingFromEditView.ReturnDate = null;

                // If no fee was entered manually, calculate it from the dates.
                if (borrowingFromEditView.IsReturned && borrowingFromEditView.LateFee == 0)
                    borrowingFromEditView.LateFee = CalculateLateFee(borrowingFromEditView.DueDate, borrowingFromEditView.ReturnDate!.Value);
            }

            if (ModelState.IsValid)
            {
                context.Borrowings.Update(borrowingFromEditView);
                context.SaveChanges();
                TempData["Success"] = "Borrowing record was updated successfully.";
                return RedirectToAction(nameof(GetIndexView));
            }

            LoadSelectLists(borrowingFromEditView.BookId, borrowingFromEditView.MemberId, includeUnavailableBookId: borrowingFromEditView.BookId);
            return View("Edit", borrowingFromEditView);
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Borrowing? borrowing = context.Borrowings.Include(b => b.Book).Include(b => b.Member).FirstOrDefault(b => b.Id == id);
            if (borrowing == null) return NotFound();
            return View("Delete", borrowing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCurrent(int id)
        {
            Borrowing? borrowing = context.Borrowings.Find(id);
            if (borrowing == null) return NotFound();

            if (!borrowing.IsReturned)
            {
                Book? book = context.Books.Find(borrowing.BookId);
                if (book != null) book.IsAvailable = true;
            }

            context.Borrowings.Remove(borrowing);
            context.SaveChanges();
            TempData["Success"] = "Borrowing record was deleted.";
            return RedirectToAction(nameof(GetIndexView));
        }

        public static decimal CalculateLateFee(DateTime dueDate, DateTime returnDate)
        {
            int lateDays = (returnDate.Date - dueDate.Date).Days;
            return lateDays > 0 ? lateDays * LateFeePerDay : 0m;
        }

        private void ValidateBorrowing(Borrowing borrowing, bool isEdit)
        {
            if (borrowing.DueDate <= borrowing.BorrowDate)
                ModelState.AddModelError(nameof(borrowing.DueDate), "Due date must be after borrow date.");

            if (borrowing.BorrowDate > DateTime.Today && !isEdit)
                ModelState.AddModelError(nameof(borrowing.BorrowDate), "Borrow date can't be in the future.");

            if (borrowing.ReturnDate.HasValue && borrowing.ReturnDate.Value < borrowing.BorrowDate)
                ModelState.AddModelError(nameof(borrowing.ReturnDate), "Return date can't be before borrow date.");
        }

        private void LoadSelectLists(int? bookId = null, int? memberId = null, int? includeUnavailableBookId = null)
        {
            var books = context.Books
                .Where(b => b.IsAvailable || b.Id == includeUnavailableBookId)
                .OrderBy(b => b.Title)
                .ToList();
            var members = context.Members.Where(m => m.IsActive).OrderBy(m => m.FullName).ToList();

            ViewBag.AllBooks = new SelectList(books, "Id", "Title", bookId);
            ViewBag.AllMembers = new SelectList(members, "Id", "FullName", memberId);
        }
    }
}
