using System;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.SampleSite.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umbraco.SampleSite.Controllers
{
    public class AccountController : SurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInModel model, string returnUrl)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var home = CurrentPage.Site();
            var usernameIsEmail = home.GetPropertyValue<bool>("usernameIsEmail");
            var loginOnSuccess = home.GetPropertyValue<bool>("loginOnSuccess");

            var registrationModel = Members.CreateRegistrationModel();
            registrationModel.Name = $"{model.FirstName} {model.Surname}";
            registrationModel.Email = model.Email;
            registrationModel.Username = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            registrationModel.UsernameIsEmail = usernameIsEmail;
            registrationModel.Password = model.Password;
            registrationModel.LoginOnSuccess = loginOnSuccess;

            var member = Members.RegisterMember(
                registrationModel,
                out MembershipCreateStatus status,
                registrationModel.LoginOnSuccess);

            switch (status)
            {
                case MembershipCreateStatus.Success:
                    return RedirectToLocal(returnUrl);
                case MembershipCreateStatus.InvalidUserName:
                    ModelState.AddModelError("Username", "Invalid username.");
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    ModelState.AddModelError("Password", "Invalid password.");
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                case MembershipCreateStatus.InvalidAnswer:
                    throw new NotImplementedException(status.ToString());
                case MembershipCreateStatus.InvalidEmail:
                    ModelState.AddModelError("Username", "Invalid email.");
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    ModelState.AddModelError("Username", "A member with this username already exists.");
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    ModelState.AddModelError("Email", "A member with this email already exists.");
                    break;
                case MembershipCreateStatus.UserRejected:
                case MembershipCreateStatus.InvalidProviderUserKey:
                case MembershipCreateStatus.DuplicateProviderUserKey:
                case MembershipCreateStatus.ProviderError:
                    ModelState.AddModelError("", $"An error occurred: {status}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return CurrentUmbracoPage();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(CurrentPage.Site().Url);
        }
    }
}
