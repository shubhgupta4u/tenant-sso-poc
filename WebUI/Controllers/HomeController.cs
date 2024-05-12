using System.Web.Mvc;
using tenant_sso_poc.WebUI.Filters;

namespace tenant_sso_poc.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
        [SsoAuthorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [SsoAuthorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}