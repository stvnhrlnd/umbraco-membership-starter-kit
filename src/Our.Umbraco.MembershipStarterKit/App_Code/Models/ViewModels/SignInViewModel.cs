using System.ComponentModel.DataAnnotations;

namespace Our.Umbraco.MembershipStarterKit.Models.ViewModels
{
    public class SignInViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string ForgotPasswordUrl { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
