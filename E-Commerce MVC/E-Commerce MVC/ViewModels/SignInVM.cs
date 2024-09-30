using System.ComponentModel.DataAnnotations;

namespace E_Commerce_MVC.ViewModels
{
    public class SignInVM
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Enter username")]
        [MaxLength(20, ErrorMessage = "Must be 6-20 characters")]
        public string Username { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
