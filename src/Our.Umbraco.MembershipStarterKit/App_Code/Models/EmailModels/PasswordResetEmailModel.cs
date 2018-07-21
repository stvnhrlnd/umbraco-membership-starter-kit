namespace Our.Umbraco.MembershipStarterKit.Models.EmailModels
{
    public class PasswordResetEmailModel
    {
        public string SiteName { get; set; }

        public string FirstName { get; set; }

        public string PasswordResetUrl { get; set; }
    }
}