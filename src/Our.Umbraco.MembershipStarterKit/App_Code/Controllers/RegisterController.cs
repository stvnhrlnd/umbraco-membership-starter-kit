using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class RegisterController : AccountController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var home = CurrentPage.Site();
            var loginOnSuccess = home.GetPropertyValue<bool>("loginOnSuccess");
            var enableConfirmationEmail = home.GetPropertyValue<bool>("enableConfirmationEmail");

            var registrationModel = Members.CreateRegistrationModel();
            registrationModel.Name = string.Format("{0} {1}", model.FirstName, model.Surname);
            registrationModel.Email = model.Email;
            registrationModel.Username = model.Username;
            registrationModel.UsernameIsEmail = false;
            registrationModel.Password = model.Password;
            registrationModel.LoginOnSuccess = loginOnSuccess;

            registrationModel.MemberProperties = new List<UmbracoProperty>
            {
                new UmbracoProperty { Alias = "firstName", Value = model.FirstName },
                new UmbracoProperty { Alias = "surname", Value = model.Surname }
            };

            MembershipCreateStatus status;
            var member = Members.RegisterMember(
                registrationModel,
                out status,
                registrationModel.LoginOnSuccess);

            switch (status)
            {
                case MembershipCreateStatus.Success:
                    if (enableConfirmationEmail)
                    {
                        SendConfirmationEmail(member.Email);
                    }
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
                    ModelState.AddModelError("", string.Format("An error occurred: {0}", status));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return CurrentUmbracoPage();
        }

        [HttpGet]
        public JsonResult IsEmailAvailable(string email)
        {
            var member = Members.GetByEmail(email);
            return Json(member == null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsUsernameAvailable(string username)
        {
            var member = Members.GetByUsername(username);
            return Json(member == null, JsonRequestBehavior.AllowGet);
        }
    }
}