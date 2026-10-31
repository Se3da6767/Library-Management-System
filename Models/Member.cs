using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to provide a valid full name.")]
        [MinLength(8)]
        [MaxLength(60)]
        [DisplayName("Full Name")]

        public string FullName { get; set; } = string.Empty;

        [Required]
        [Length(14, 14, ErrorMessage = "National Id must be 14 characters.")]
        [DisplayName("National Id")]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        [Compare(nameof(Email), ErrorMessage = "Email and ConfirmEmail do not match.")]
        [DisplayName("Confirm Email")]
        public string ConfirmEmail { get; set; } = string.Empty;

        [RegularExpression("^01\\d{9}$", ErrorMessage = "Invalid phone number.")]
        [DisplayName("Phone Number")]
        public string PhoneNo { get; set; } = string.Empty;

        [MaxLength(120)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Join Date")]
        public DateTime JoinDate { get; set; }

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [MaxLength(20)]
        [DisplayName("Membership Type")]
        public string MembershipType { get; set; } = "Standard";

        [ValidateNever]
        public ICollection<Borrowing> Borrowings { get; set; } = new List<Borrowing>();
    }
}
