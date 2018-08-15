using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class ResetPasswordController : BaseSurfaceController
    {
        [ChildActionOnly]
        public ActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return PartialView("_Error");
            }

            var member = GetMemberByPasswordResetToken(token);
            if (member == null || !IsPasswordResetTokenValid(member))
            {
                return PartialView("_Error");
            }

            var model = new ResetPasswordViewModel { Token = token };
            return PartialView("Forms/_ResetPasswordForm", model);
        }

        [HttpPost]
        [NotChildAction]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var member = GetMemberByPasswordResetToken(model.Token);
            if (member == null || !IsPasswordResetTokenValid(member))
            {
                ModelState.AddModelError("", "Invalid or expired password reset token.");
                return CurrentUmbracoPage();
            }

            member.SetValue("passwordResetToken", null);
            member.SetValue("passwordResetTokenExpiry", null);
            Services.MemberService.Save(member);
            Services.MemberService.SavePassword(member, model.Password);

            var signIn = CurrentPage.Site()
                .Children.First(x => x.DocumentTypeAlias == "signIn");
            return RedirectToUmbracoPage(signIn);
        }

        private IMember GetMemberByPasswordResetToken(string token)
        {
            return Services.MemberService
                .GetMembersByPropertyValue("passwordResetToken", token)
                .FirstOrDefault();
        }

        private bool IsPasswordResetTokenValid(IMember member)
        {
            var passwordResetTokenExpiry = member.GetValue<DateTime?>("passwordResetTokenExpiry");
            return passwordResetTokenExpiry.HasValue && DateTime.Now <= passwordResetTokenExpiry;
        }
    }
}