using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class SignInController : BaseSurfaceController
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
        [NotChildAction]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (!Membership.ValidateUser(model.Username, model.Password))
            {
                Alert("danger", "Invalid login attempt.");
                return CurrentUmbracoPage();
            }

            var member = Members.GetByUsername(model.Username);
            if (!member.HasValue("confirmationDate"))
            {
                Alert("danger", "Please confirm your email address to sign in.");
                return CurrentUmbracoPage();
            }

            FormsAuthentication.SetAuthCookie(model.Username, true);
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