using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System;
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

            var member = Services.MemberService.GetByUsername(model.Username);
            if (member == null)
            {
                Alert("danger", "Invalid login attempt.");
                return CurrentUmbracoPage();
            }

            var confirmationDate = member.GetValue<DateTime?>("confirmationDate");
            if (!confirmationDate.HasValue)
            {
                TempData["UnconfirmedMember"] = model.Username;
                return CurrentUmbracoPage();
            }

            if (member.IsLockedOut)
            {
                Alert("danger", "Your account has been locked due to several failed password attempts. You can unlock your account by resetting your password.");
                return CurrentUmbracoPage();
            }

            if (!Membership.ValidateUser(model.Username, model.Password))
            {
                Alert("danger", "Invalid login attempt.");
                return CurrentUmbracoPage();
            }

            FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe);
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