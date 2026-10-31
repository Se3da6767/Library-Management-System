using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Borrowing
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Borrow Date")]
        public DateTime BorrowDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Due Date")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Return Date")]
        public DateTime? ReturnDate { get; set; }

        [DisplayName("Is Returned")]
        public bool IsReturned { get; set; }

        [DisplayName("Book")]
        public int BookId { get; set; }

        [ValidateNever]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Book Book { get; set; } = null!;

        [DisplayName("Member")]
        public int MemberId { get; set; }

        [Range(0, 10000, ErrorMessage = "Late fee must be between 0 and 10000.")]
        [DisplayName("Late Fee")]
        public decimal LateFee { get; set; }

        [ValidateNever]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Member Member { get; set; } = null!;
    }
}
