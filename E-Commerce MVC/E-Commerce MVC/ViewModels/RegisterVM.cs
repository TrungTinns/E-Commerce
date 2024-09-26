using E_Commerce_MVC.Data;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ViewModels
{
    public class RegisterVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "*")]
        [MaxLength(20, ErrorMessage = "Must be 6-20 characters")]
        public string UserID { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Fullname")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Must be 50 characters")]
        public string FullName { get; set; }

        public bool Gender { get; set; } = true;

        [Display(Name = "Birth date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Address")]
        [MaxLength(60, ErrorMessage = "Must be 60 characters")]
        public string Address { get; set; }

        [Display(Name = "Phone number")]
        [MaxLength(10, ErrorMessage = "Must be 10 numbers")]
        [RegularExpression(@"0[987654321]\d{8}", ErrorMessage = "Invalid phone number")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        public string? Image { get; set; }
    }
}
