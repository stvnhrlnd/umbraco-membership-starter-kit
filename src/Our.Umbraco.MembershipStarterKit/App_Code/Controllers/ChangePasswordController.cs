using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class ChangePasswordController : BaseSurfaceController
    {
        [HttpPost]
        [NotChildAction]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var currentMemberProfile = Members.GetCurrentMemberProfileModel();
            if (!Membership.ValidateUser(currentMemberProfile.UserName, model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "Password incorrect.");
                return CurrentUmbracoPage();
            }

            var member = Services.MemberService.GetByUsername(currentMemberProfile.UserName);
            Services.MemberService.SavePassword(member, model.NewPassword);

            Alert("success", "Password changed.");
            var profilePage = CurrentPage.Site().Children
                .First(x => x.DocumentTypeAlias == "profile");
            return RedirectToUmbracoPage(profilePage);
        }
    }
}