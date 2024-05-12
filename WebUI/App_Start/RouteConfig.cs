using System.Web.Mvc;
using System.Web.Routing;

namespace tenant_sso_poc.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{tenantalias}/{controller}/{action}/{id}",
                defaults: new { tenantalias="mordor", controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
