using System.Web.Mvc;
using Umbraco.SampleSite.Models.ViewModels;

namespace Umbraco.SampleSite.Controllers
{
    public class SignInController : AccountController
    {
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