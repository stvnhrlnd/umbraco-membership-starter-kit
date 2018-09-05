using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class EditProfileController : BaseSurfaceController
    {
        [ChildActionOnly]
        public ActionResult EditProfile()
        {
            if (!Members.IsLoggedIn())
            {
                return PartialView("_Error");
            }

            var member = Members.GetCurrentMember();
            var model = new EditProfileViewModel
            {
                FirstName = member.GetPropertyValue<string>("firstName"),
                Surname = member.GetPropertyValue<string>("surname"),
                Email = member.GetPropertyValue<string>("Email"),
                Username = member.GetPropertyValue<string>("UserName"),
                Bio = member.GetPropertyValue<string>("bio")
            };
            return PartialView("Forms/_EditProfileForm", model);
        }

        [HttpPost]
        [NotChildAction]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var memberId = Members.GetCurrentMemberId();
            var member = Services.MemberService.GetById(memberId);
            var oldUsername = member.Username;

            member.Name = string.Format("{0} {1}", model.FirstName, model.Surname);
            member.Email = model.Email;
            member.Username = model.Username;
            member.SetValue("firstName", model.FirstName);
            member.SetValue("surname", model.Surname);
            member.SetValue("bio", model.Bio);
            Services.MemberService.Save(member);

            if (member.Username != oldUsername)
            {
                var authCookie = Request.Cookies.Get(FormsAuthentication.FormsCookieName);
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                FormsAuthentication.SetAuthCookie(member.Username, authTicket.IsPersistent);
            }

            Alert("success", "Profile updated.");
            var profilePage = CurrentPage.Site().Children
                .First(x => x.DocumentTypeAlias == "profile");
            return RedirectToUmbracoPage(profilePage);
        }

        [HttpGet]
        public JsonResult IsEmailAvailable(string email)
        {
            var currentMemberProfile = Members.GetCurrentMemberProfileModel();
            if (currentMemberProfile.Email == email)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            var member = Members.GetByEmail(email);
            return Json(member == null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsUsernameAvailable(string username)
        {
            var currentMemberProfile = Members.GetCurrentMemberProfileModel();
            if (currentMemberProfile.UserName == username)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            var member = Members.GetByUsername(username);
            return Json(member == null, JsonRequestBehavior.AllowGet);
        }
    }
}