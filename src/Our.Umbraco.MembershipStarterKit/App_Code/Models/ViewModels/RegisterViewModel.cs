using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsEmailAvailable", "Register", ErrorMessage = "A member with this email already exists.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9_]{1,15}$")]
        [Remote("IsUsernameAvailable", "Register", ErrorMessage = "A member with this username already exists.")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
