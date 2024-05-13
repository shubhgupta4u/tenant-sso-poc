using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System.Collections.Generic;
using tenant_sso_poc.models;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.Interfaces;
using tenant_sso_poc.WebUI.Security;

[assembly: OwinStartup(typeof(tenant_sso_poc.WebUI.Startup))]
namespace tenant_sso_poc.WebUI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ITenantRepository tenantRepository = new TenantRepository();
            List<Tenant> tenants = new List<Tenant>(tenantRepository.GetAllAsync().Result);
            SetCookieAuthenticationAsDefault(app);
            app.UseSsoOauthAuthentication(tenants);
        }
        private void SetCookieAuthenticationAsDefault(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
        }
    }
    
    
    
    
   

   
   
    
    
   
    
}