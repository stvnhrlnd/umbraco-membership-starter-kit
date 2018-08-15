using Our.Umbraco.MembershipStarterKit.Models.EmailModels;
using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Web;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class ForgotPasswordController : BaseSurfaceController
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            if (Members.GetByEmail(model.Email) != null)
            {
                SendPasswordResetEmail(model.Email);
            }
            
            return Redirect("/");
        }

        private void SendPasswordResetEmail(string email)
        {
            var member = Services.MemberService.GetByEmail(email);
            var passwordResetToken = Guid.NewGuid().ToString();
            member.SetValue("passwordResetToken", passwordResetToken);
            member.SetValue("passwordResetTokenExpiry", DateTime.Now.AddMinutes(10));
            Services.MemberService.Save(member);

            var home = CurrentPage.Site();
            var siteName = home.GetPropertyValue<string>("siteName");
            var resetPasswordPage = home.Children
                .First(x => x.DocumentTypeAlias == "resetPassword");
            var resetPasswordUrl = string.Format(
                "{0}?token={1}",
                resetPasswordPage.UrlWithDomain(),
                passwordResetToken);

            var emailSender = new EmailSender();
            var mailMessage = new MailMessage
            {
                Subject = string.Format("{0} - Reset your password", siteName),
                IsBodyHtml = true,
                Body = RenderPartial("Emails/_PasswordResetEmail", new PasswordResetEmailModel
                {
                    SiteName = siteName,
                    FirstName = member.GetValue<string>("firstName"),
                    PasswordResetUrl = resetPasswordUrl
                })
            };
            mailMessage.To.Add(email);
            emailSender.Send(mailMessage);
        }
    }
}