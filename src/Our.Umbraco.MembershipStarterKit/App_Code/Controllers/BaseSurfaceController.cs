using Our.Umbraco.MembershipStarterKit.Models.ViewModels;
using System.IO;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace Our.Umbraco.MembershipStarterKit.Controllers
{
    public abstract class BaseSurfaceController : SurfaceController
    {
        protected void Alert(string type, string text)
        {
            TempData["Alert"] = new AlertViewModel { Type = type, Text = text };
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect(CurrentPage.Site().Url);
        }

        protected string RenderPartial(string name, object model)
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
