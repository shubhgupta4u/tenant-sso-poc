using System.Web.Http;

namespace tenant_sso_poc.WebUI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{tenantalias}/api/{controller}/{id}",
                defaults: new { tenantalias = "mordor", controller = "DataSeeder", id = RouteParameter.Optional }
            );
        }
    }
}
