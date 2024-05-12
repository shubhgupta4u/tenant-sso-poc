using System.Threading;
using System.Web;
using System.Web.Mvc.Filters;

namespace tenant_sso_poc.WebUI.Filters
{
    public class TenantRoutingFilter : IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            string tenantAlias = Constants.DEFAULT_TENANT_ALIAS;
            object keyValue;
            if(filterContext.RouteData.Values.ContainsKey(Constants.TENANT_ROUTE_KEY) && filterContext.RouteData.Values.TryGetValue(Constants.TENANT_ROUTE_KEY,out keyValue) && keyValue != null && !keyValue.ToString().Equals("home"))
            {
                tenantAlias = keyValue.ToString();
            }
            filterContext.HttpContext.GetOwinContext().Set(Constants.TENANT_ROUTE_KEY, tenantAlias);
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
        }
    }
}