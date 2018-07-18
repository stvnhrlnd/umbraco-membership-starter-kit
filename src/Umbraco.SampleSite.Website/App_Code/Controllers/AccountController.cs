﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Core;
using Umbraco.SampleSite.Models;
using Umbraco.Web;
using Umbraco.Web.Models;
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
        public ActionResult Register(Models.RegisterModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            var home = CurrentPage.Site();
            var usernameIsEmail = home.GetPropertyValue<bool>("usernameIsEmail");
            var loginOnSuccess = home.GetPropertyValue<bool>("loginOnSuccess");
            var enableConfirmationEmail = home.GetPropertyValue<bool>("enableConfirmationEmail");

            var registrationModel = Members.CreateRegistrationModel();
            registrationModel.Name = string.Format("{0} {1}", model.FirstName, model.Surname);
            registrationModel.Email = model.Email;
            registrationModel.Username = string.IsNullOrWhiteSpace(model.Username) ? model.Email : model.Username;
            registrationModel.UsernameIsEmail = usernameIsEmail;
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
                    if (usernameIsEmail)
                    {
                        ModelState.AddModelError("Email", "A member with this email already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError("Username", "A member with this username already exists.");
                    }
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

        [ChildActionOnly]
        public ActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return PartialView("_Error");
            }
            else
            {
                var member = Services.MemberService
                    .GetMembersByPropertyValue("confirmationToken", token)
                    .FirstOrDefault();

                if (member == null)
                {
                    return PartialView("_Error");
                }
                else
                {
                    member.SetValue("confirmationToken", null);
                    member.SetValue("confirmationDate", DateTime.Now);
                    Services.MemberService.Save(member);
                }
            }

            return PartialView("_ConfirmEmail");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(CurrentPage.Site().Url);
        }

        private void SendConfirmationEmail(string email)
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
                Body = RenderPartial("_ConfirmationEmail", new ConfirmationEmailModel
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
