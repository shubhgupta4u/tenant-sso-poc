using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using tenant_sso_poc.WebUI;
using tenant_sso_poc.WebUI.Filters;

namespace WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = DiBootstrapper.Instance.RegisterDependenciesAsync().Result;
            GlobalFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
