using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace tenant_sso_poc.WebUI
{
    public class BaseController: Controller
    {
        public string TenantAlias { 
            get
            {
                return HttpContext.GetOwinContext().Get<string>(Constants.TENANT_ROUTE_KEY);
            }
        }
        public BaseController()
        {
            
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.TenantAlias = this.TenantAlias;
        }

    }
}