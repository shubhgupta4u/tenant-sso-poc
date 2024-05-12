using System;
using System.Web.Mvc;

namespace tenant_sso_poc.WebUI.Filters
{
    public class GlobalFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new TenantRoutingFilter());
        }
    }

}