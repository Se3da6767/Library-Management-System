using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Models
{
    public class Publisher
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid publisher name.")]
        [MinLength(2, ErrorMessage = "Publisher name mustn't be less than 2 characters.")]
        [MaxLength(60, ErrorMessage = "Publisher name mustn't exceed 60 characters.")]
        [DisplayName("Publisher Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "You have to provide a valid country.")]
        [MinLength(2)]
        [MaxLength(40)]
        public string Country { get; set; } = string.Empty;

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression("^01\\d{9}$", ErrorMessage = "Invalid phone number.")]
        [DisplayName("Phone Number")]
        public string PhoneNo { get; set; } = string.Empty;

        [ValidateNever]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
