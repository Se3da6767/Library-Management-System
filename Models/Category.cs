using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid category name.")]
        [MinLength(2, ErrorMessage = "Category name mustn't be less than 2 characters.")]
        [MaxLength(40, ErrorMessage = "Category name mustn't exceed 40 characters.")]
        [DisplayName("Category Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "You have to provide a valid description.")]
        [MinLength(5, ErrorMessage = "Description mustn't be less than 5 characters.")]
        [MaxLength(150, ErrorMessage = "Description mustn't exceed 150 characters.")]
        public string Description { get; set; } = string.Empty;

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [ValidateNever]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
