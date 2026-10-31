using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid book title.")]
        [MinLength(2, ErrorMessage = "Title mustn't be less than 2 characters.")]
        [MaxLength(100, ErrorMessage = "Title mustn't exceed 100 characters.")]
        [DisplayName("Book Title")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "You have to provide a valid author name.")]
        [MinLength(4)]
        [MaxLength(80)]
        [DisplayName("Author Name")]
        public string AuthorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "You have to provide a valid ISBN.")]
        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must contain 10 or 13 digits.")]
        public string ISBN { get; set; } = string.Empty;

        [Range(1, 100000, ErrorMessage = "Price must be between 1 and 100000.")]
        public decimal Price { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Publication Date")]
        public DateTime PublicationDate { get; set; }

        [Range(1, 10000)]
        public int Pages { get; set; }
        [Range(0, 1000)]
        [DisplayName("Stock Quantity")]
        public int StockQuantity { get; set; }

        [Required]
        [MaxLength(30)]
        public string Language { get; set; } = string.Empty;

        [DisplayName("Is Available")]
        public bool IsAvailable { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [ValidateNever]
        public string? ImagePath { get; set; }

        [NotMapped]
        [DisplayName("Book Cover")]
        public IFormFile? ImageFile { get; set; }

        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Category Category { get; set; } = null!;

        [DisplayName("Publisher")]
        public int PublisherId { get; set; }

        [ValidateNever]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Publisher Publisher { get; set; } = null!;

        [ValidateNever]
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}
