using System.Web.Mvc;
using Umbraco.SampleSite.Models;
using Umbraco.Web.Mvc;

namespace Umbraco.SampleSite.Controllers
{
    public class AccountController : SurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(SignInModel model)
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

            return Redirect("/");
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
