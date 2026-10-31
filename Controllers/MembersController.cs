using LibrarySystem.Data;
using LibrarySystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    public class MembersController : Controller
    {
        private readonly AppDbContext context;

        public MembersController(AppDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetIndexView(string? search)
        {
            IQueryable<Member> members = context.Members;
            if (!string.IsNullOrWhiteSpace(search))
                members = members.Where(m => m.FullName.Contains(search) || m.Email.Contains(search) || m.PhoneNo.Contains(search));

            ViewBag.CurrentSearch = search;
            return View("Index", members.Include(m => m.Borrowings).OrderBy(m => m.FullName).ToList());
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(GetIndexView));

        [HttpGet]
        public IActionResult GetDetailsView(int id)
        {
            Member? member = context.Members
                .Include(m => m.Borrowings)
                .ThenInclude(b => b.Book)
                .FirstOrDefault(m => m.Id == id);
            if (member == null) return NotFound();
            return View("Details", member);
        }

        [HttpGet]
        public IActionResult GetCreateView() => View("Create", new Member { JoinDate = DateTime.Today, IsActive = true });

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddNew(Member memberFromCreateView)
        {
            if (memberFromCreateView.BirthDate.AddYears(12) > DateTime.Now)
                ModelState.AddModelError(nameof(memberFromCreateView.BirthDate), "Member must be at least 12 years old.");

            if (context.Members.Any(m => m.NationalId == memberFromCreateView.NationalId))
                ModelState.AddModelError(nameof(memberFromCreateView.NationalId), "Duplicate National Id.");

            if (context.Members.Any(m => m.Email == memberFromCreateView.Email))
                ModelState.AddModelError(nameof(memberFromCreateView.Email), "Duplicate email address.");

            if (ModelState.IsValid)
            {
                context.Members.Add(memberFromCreateView);
                context.SaveChanges();
                TempData["Success"] = $"{memberFromCreateView.FullName} was added as a new member.";
                return RedirectToAction(nameof(GetIndexView));
            }
            return View("Create", memberFromCreateView);
        }

        [HttpGet]
        public IActionResult GetEditView(int id)
        {
            Member? member = context.Members.Find(id);
            if (member == null) return NotFound();
            member.ConfirmEmail = member.Email;
            return View("Edit", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCurrent(Member memberFromEditView)
        {
            if (context.Members.Any(m => m.NationalId == memberFromEditView.NationalId && m.Id != memberFromEditView.Id))
                ModelState.AddModelError(nameof(memberFromEditView.NationalId), "Duplicate National Id.");

            if (context.Members.Any(m => m.Email == memberFromEditView.Email && m.Id != memberFromEditView.Id))
                ModelState.AddModelError(nameof(memberFromEditView.Email), "Duplicate email address.");

            if (memberFromEditView.BirthDate.AddYears(12) > DateTime.Now)
                ModelState.AddModelError(nameof(memberFromEditView.BirthDate), "Member must be at least 12 years old.");

            if (ModelState.IsValid)
            {
                context.Members.Update(memberFromEditView);
                context.SaveChanges();
                TempData["Success"] = $"{memberFromEditView.FullName}'s data was updated.";
                return RedirectToAction(nameof(GetIndexView));
            }
            return View("Edit", memberFromEditView);
        }

        [HttpGet]
        public IActionResult GetDeleteView(int id)
        {
            Member? member = context.Members.Include(m => m.Borrowings).FirstOrDefault(m => m.Id == id);
            if (member == null) return NotFound();
            return View("Delete", member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCurrent(int id)
        {
            Member? member = context.Members.Include(m => m.Borrowings).FirstOrDefault(m => m.Id == id);
            if (member == null) return NotFound();

            // Borrowings reference the member with NoAction, so any history (even returned books) blocks deletion.
            if (member.Borrowings.Any(b => !b.IsReturned))
            {
                ModelState.AddModelError(string.Empty, "This member has an active borrowing and can't be deleted.");
                return View("Delete", member);
            }

            if (member.Borrowings.Any())
            {
                ModelState.AddModelError(string.Empty, "This member has borrowing history and can't be deleted. You can mark the member as inactive instead.");
                return View("Delete", member);
            }

            context.Members.Remove(member);
            context.SaveChanges();
            TempData["Success"] = $"{member.FullName} was deleted.";
            return RedirectToAction(nameof(GetIndexView));
        }
    }
}
