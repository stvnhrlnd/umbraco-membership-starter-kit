namespace Umbraco.SampleSite.Models
{
    public class ConfirmEmailModel
    {
        public bool TokenEmpty { get; set; }

        public bool TokenInvalid { get; set; }
    }
}