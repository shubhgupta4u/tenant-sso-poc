using Microsoft.Owin.Security.Cookies;
using System.Web;
using System.Web.Mvc;

namespace tenant_sso_poc.WebUI.Controllers
{
    public class AuthenticationController : BaseController
    {
        public ActionResult SignIn()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                Constants.DefaultAuthenticationType);
                return new HttpUnauthorizedResult();
            }
            return RedirectToAction("Index", "Home", this.TenantAlias);
        }
        public ActionResult SignOut()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    CookieAuthenticationDefaults.AuthenticationType,
                    Constants.DefaultAuthenticationType);
            }
            return RedirectToAction("PostSignOut");
        }
        public ActionResult PostSignOut()
        {
            return RedirectToAction("Index", "Home", this.TenantAlias);
        }
    }
}