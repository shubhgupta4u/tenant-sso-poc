using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tenant_sso_poc.WebUI.Models.Oauth
{
    public class TenantAuthenticationOptions
    {
        public string TenantAlias { get; set; }
        public string ClientSecret { get; set; }
        public string ClientId { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public string LogoutUri { get; set; }
        public WsdlConfig WsdlConfig { get; set; }
        public ISecureDataFormat<AuthenticationProperties> StateDataFormat { get; set; }
    }
}