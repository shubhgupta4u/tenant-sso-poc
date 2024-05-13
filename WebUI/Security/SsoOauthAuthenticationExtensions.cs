using Owin;
using System.Collections.Generic;
using tenant_sso_poc.models;
using tenant_sso_poc.WebUI.Middleware;

namespace tenant_sso_poc.WebUI.Security
{
    public static class SsoOauthAuthenticationExtensions
    {
        public static IAppBuilder UseSsoOauthAuthentication(this IAppBuilder app, List<Tenant> tenants)
        {
            SsoOauthAuthenticationOptions oauthAuthenticationOptions = new SsoOauthAuthenticationOptions(tenants);
            return app.Use(typeof(SsoOauthAuthenticationMiddleware), app, oauthAuthenticationOptions);
        }
    }
}