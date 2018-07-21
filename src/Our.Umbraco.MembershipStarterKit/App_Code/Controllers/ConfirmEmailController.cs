using System;
using System.Linq;
using System.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public class ConfirmEmailController : AccountController
    {
        [ChildActionOnly]
        public ActionResult ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return PartialView("_Error");
            }

            var member = Services.MemberService
                .GetMembersByPropertyValue("confirmationToken", token)
                .FirstOrDefault();

            if (member == null)
            {
                return PartialView("_Error");
            }

            member.SetValue("confirmationToken", null);
            member.SetValue("confirmationDate", DateTime.Now);
            Services.MemberService.Save(member);

            return PartialView("_ConfirmEmail");
        }
    }
}