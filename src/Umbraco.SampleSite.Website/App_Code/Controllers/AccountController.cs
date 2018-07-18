using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.SampleSite.Models.EmailModels;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Umbraco.SampleSite.Controllers
{
    public class AccountController : SurfaceController
    {
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(CurrentPage.Site().Url);
        }

        protected void SendConfirmationEmail(string email)
        {
            var member = Services.MemberService.GetByEmail(email);
            var confirmationToken = Guid.NewGuid().ToString();
            member.SetValue("confirmationToken", confirmationToken);
            Services.MemberService.Save(member);

            var home = CurrentPage.Site();
            var siteName = home.GetPropertyValue<string>("siteName");
            var confirmEmailPage = home.Children
                .Where(x => x.DocumentTypeAlias == "confirmEmail")
                .First();
            var confirmEmailUrl = string.Format(
                "{0}?token={1}",
                confirmEmailPage.UrlWithDomain(),
                confirmationToken);

            var emailSender = new EmailSender();
            var mailMessage = new MailMessage
            {
                Subject = string.Format("Welcome to {0} - Please confirm your email address", siteName),
                IsBodyHtml = true,
                Body = RenderPartial("Emails/_ConfirmationEmail", new ConfirmationEmailModel
                {
                    SiteName = siteName,
                    FirstName = member.GetValue<string>("firstName"),
                    ConfirmEmailUrl = confirmEmailUrl
                })
            };
            mailMessage.To.Add(email);
            emailSender.Send(mailMessage);
        }

        private string RenderPartial(string name, object model)
        {
            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, name);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    new ViewDataDictionary { Model = model },
                    new TempDataDictionary(),
                    stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                return stringWriter.GetStringBuilder().ToString();
            }
        }
    }
}
