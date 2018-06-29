using System.ComponentModel.DataAnnotations;

namespace Umbraco.SampleSite.Models
{
    public class SignInModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
