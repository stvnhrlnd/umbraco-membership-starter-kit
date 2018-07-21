using System.ComponentModel.DataAnnotations;

namespace Our.Umbraco.MembershipStarterKit.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}