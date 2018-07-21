namespace Our.Umbraco.MembershipStarterKit.Models.EmailModels
{
    public class ConfirmationEmailModel
    {
        public string SiteName { get; set; }

        public string FirstName { get; set; }

        public string ConfirmEmailUrl { get; set; }
    }
}