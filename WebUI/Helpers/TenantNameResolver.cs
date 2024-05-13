using System.Linq;
using System.Web;

namespace tenant_sso_poc.WebUI.Helpers
{
    public class TenantNameResolver
    {
        public static string Get(HttpContext context)
        {
            if (context.Request.RawUrl.ToCharArray().Count(s => s == '/') > 1)
            {
                return context.Request.RawUrl.Split('/')[1];
            }
            else
            {
                return Constants.DEFAULT_TENANT_ALIAS;
            }
        }
    }
}