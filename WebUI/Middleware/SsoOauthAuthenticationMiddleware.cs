using Microsoft.Owin;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Owin;
using tenant_sso_poc.WebUI.Security;

namespace tenant_sso_poc.WebUI.Middleware
{
    public class SsoOauthAuthenticationMiddleware : AuthenticationMiddleware<SsoOauthAuthenticationOptions>
    {

        public SsoOauthAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, SsoOauthAuthenticationOptions options)
            : base(next, options)
        {
            foreach (var identitySetting in options.TenantIdentitySettings)
            {
                if (identitySetting.Value.StateDataFormat == null)
                {
                    var dataProtector = app.CreateDataProtector(typeof(SsoOauthAuthenticationMiddleware).FullName,
                        options.AuthenticationType);

                    identitySetting.Value.StateDataFormat = new PropertiesDataFormat(dataProtector);
                }
            }

        }

        // Called for each request, to create a handler for each request.
        protected override AuthenticationHandler<SsoOauthAuthenticationOptions> CreateHandler()
        {
            return new SsoOauthAuthenticationHandler();
        }
    }
}