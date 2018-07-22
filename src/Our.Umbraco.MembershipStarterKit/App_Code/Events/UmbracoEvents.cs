using ClientDependency.Core;
using Umbraco.Core;

namespace Our.Umbraco.MembershipStarterKit.Events
{
    public class UmbracoEvents : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            CreateBundles();
        }

        private void CreateBundles()
        {
            BundleManager.CreateCssBundle(
                "MasterCss",
                1,
                new CssFile("~/css/bootstrap.css"),
                new CssFile("~/css/site.css"));

            BundleManager.CreateJsBundle(
                "MasterScripts",
                1,
                new JavascriptFile("~/scripts/jquery-3.3.1.js"),
                new JavascriptFile("~/scripts/popper.js"),
                new JavascriptFile("~/scripts/bootstrap.js"),
                new JavascriptFile("~/scripts/site.js"));

            BundleManager.CreateJsBundle(
                "ValidationScripts",
                new JavascriptFile("~/scripts/jquery.validate.js"),
                new JavascriptFile("~/scripts/jquery.validate.unobtrusive.options.js"),
                new JavascriptFile("~/scripts/jquery.validate.unobtrusive.js"));
        }
    }
}
