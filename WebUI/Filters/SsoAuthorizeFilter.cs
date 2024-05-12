using System.Web;
using System.Web.Mvc;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.WebUI.Filters
{
    public class SsoAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly IUserRepository _userRepository;
        public SsoAuthorizeAttribute()
        {
            
        }
        ///<inheritdoc/>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
        ///<inheritdoc/>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthenticated = base.AuthorizeCore(httpContext);
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                isAuthenticated = false;
            }
            return isAuthenticated;
        }
        ///<inheritdoc/>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            base.HandleUnauthorizedRequest(filterContext);
        }
        ///<inheritdoc/>
        protected override HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            return base.OnCacheAuthorization(httpContext);
        }
    }
}