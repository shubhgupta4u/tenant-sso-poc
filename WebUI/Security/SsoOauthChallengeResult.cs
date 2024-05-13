using Microsoft.Owin.Security;
using System.Web;
using System.Web.Mvc;

namespace tenant_sso_poc.WebUI.Security
{
    /// <summary>
    /// Custom class to triger 401.  
    /// </summary>
    public class SsoOauthChallengeResult : HttpUnauthorizedResult
    {
        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }

        public SsoOauthChallengeResult(string redirectUri = "/")
        {
            LoginProvider = Constants.LoginProvider;
            RedirectUri = redirectUri;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }

    }
}