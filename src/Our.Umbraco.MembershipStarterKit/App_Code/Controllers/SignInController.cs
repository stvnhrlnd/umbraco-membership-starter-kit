using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Web;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class SignInController : AccountController
    {
        [ChildActionOnly]
        public ActionResult SignIn()
        {
            var model = new SignInViewModel();

            var forgotPasswordPage = CurrentPage.Site().Children
                .FirstOrDefault(x => x.DocumentTypeAlias == "forgotPassword");
            if (forgotPasswordPage != null)
            {
                model.ForgotPasswordUrl = forgotPasswordPage.Url;
            }

            return PartialView("Forms/_SignInForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!Members.Login(model.Username, model.Password))
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return CurrentUmbracoPage();
            }

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            Members.Logout();
            return Redirect("/");
        }
    }
}