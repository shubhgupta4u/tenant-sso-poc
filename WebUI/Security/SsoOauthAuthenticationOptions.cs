using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using tenant_sso_poc.models;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.Interfaces;
using tenant_sso_poc.WebUI.Helpers;
using tenant_sso_poc.WebUI.Models.Oauth;

namespace tenant_sso_poc.WebUI.Security
{
    public class SsoOauthAuthenticationOptions : AuthenticationOptions
    {
        public SsoOauthAuthenticationOptions(List<Tenant> tenants)
            : base(Constants.DefaultAuthenticationType)
        {
            Description.Caption = Constants.DefaultAuthenticationType;
            AuthenticationMode = AuthenticationMode.Passive;
            TenantIdentitySettings = new Dictionary<string, TenantAuthenticationOptions>();
            IIdentitySettingRepository repository = new IdentitySettingRespository();
            List<IdentitySetting> identitySettings = new List<IdentitySetting>(repository.GetAllAsync().Result);
            foreach (Tenant tenant in tenants)
            {
                string tenantAlias = tenant.Alias.ToLower();
                if (!TenantIdentitySettings.ContainsKey(tenantAlias))
                {
                    IdentitySetting identitySetting = identitySettings.FirstOrDefault(s => s.Id == tenant.IdentityId);
                    if (identitySetting != null)
                    {
                        WsdlConfig config = WsdlConfigurationProvider.GetAsync(identitySetting.AuthorityUrl).Result;
                        TenantAuthenticationOptions authenticationOption = new TenantAuthenticationOptions()
                        {
                            ClientId = identitySetting.ClientId,
                            ClientSecret = identitySetting.ClientSecret,
                            RedirectUri = ConfigurationManager.AppSettings[tenantAlias + ":RedirectUri"],
                            PostLogoutRedirectUri = ConfigurationManager.AppSettings[tenantAlias + ":PostLogoutRedirectUri"],
                            LogoutUri = ConfigurationManager.AppSettings[tenantAlias + ":LogoutUri"],
                            TenantAlias = tenantAlias,
                            WsdlConfig = config
                        };
                        TenantIdentitySettings.Add(tenantAlias, authenticationOption);
                    }
                }
            }

        }
        public IDictionary<string, TenantAuthenticationOptions> TenantIdentitySettings { get; set; }

    }
}