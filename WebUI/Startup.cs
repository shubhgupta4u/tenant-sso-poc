using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OpenIdConnect;
using Okta.AspNet;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using tenant_sso_poc.models;
using tenant_sso_poc.repository;
using tenant_sso_poc.repository.Interfaces;
using System.Linq;
using tenant_sso_poc.repository.DataSeeder;

[assembly: OwinStartup(typeof(tenant_sso_poc.WebUI.Startup))]
namespace tenant_sso_poc.WebUI
{
    public class Startup
    {
        private void createDummyData()
        {
            //ITenantRepository tenantRepository = new TenantRepository();
            //TenantSeed tenantSeed = new TenantSeed(tenantRepository);
            //tenantSeed.StartAsync().Wait();
            IIdentitySettingRepository identitySettingRepository = new IdentitySettingRespository();
            //IdentitySettingSeed settingSeed = new IdentitySettingSeed(identitySettingRepository);
            //settingSeed.StartAsync().Wait();

            IdentitySetting setting = new IdentitySetting() { Id = tenant_sso_poc.data.Constants.MORDOR_AUTHORITYID, AuthorityUrl = @"https://dev-35555547.okta.com", ClientId = "0oah0xsjup3bsxJUU5d7", ClientSecret = "QFhhK8fCeNNPhN2WWgqwEv1NpSFwu2t7k9lVjgLwNrDKZ4i53JELM-L0BrqupYNh", SupportLocale = true };
            identitySettingRepository.UpdateAsync(setting).Wait();
        }
        public void Configuration(IAppBuilder app)
        {
            createDummyData();
            ITenantRepository tenantRepository = new TenantRepository();
            List<Tenant> tenants = new List<Tenant>(tenantRepository.GetAllAsync().Result);
            SetCookieAuthenticationAsDefault(app);
            //AddOktaAuthenticationMiddleware(app);
            //AddOauthAuthenticationMiddleware(app);
            app.UseSsoOauthAuthentication(tenants);
        }
        private void SetCookieAuthenticationAsDefault(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
        }
        private void AddOktaAuthenticationMiddleware(IAppBuilder app)
        {
            app.UseOktaMvc(new OktaMvcOptions()
            {
                OktaDomain = ConfigurationManager.AppSettings["got:OktaDomain"],
                ClientId = ConfigurationManager.AppSettings["got:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["got:ClientSecret"],
                RedirectUri = ConfigurationManager.AppSettings["got:RedirectUri"],
                PostLogoutRedirectUri = ConfigurationManager.AppSettings["got:PostLogoutRedirectUri"],
                GetClaimsFromUserInfoEndpoint = true,
                Scope = new List<string> { "openid", "profile", "email" },
            });
        }
        private void AddOauthAuthenticationMiddleware(IAppBuilder app)
        {
            app.UseOpenIdConnectAuthentication(
                 new OpenIdConnectAuthenticationOptions
                 {
                     ClientId = ConfigurationManager.AppSettings["got:ClientId"],
                     Authority = ConfigurationManager.AppSettings["got:OktaDomain"],
                     ClientSecret = ConfigurationManager.AppSettings["got:ClientSecret"],
                     RedirectUri = ConfigurationManager.AppSettings["got:RedirectUri"],
                     PostLogoutRedirectUri = ConfigurationManager.AppSettings["got:PostLogoutRedirectUri"],
                     CallbackPath = new PathString(ConfigurationManager.AppSettings["got:callbackPath"]),
                     Notifications = new OpenIdConnectAuthenticationNotifications()
                     {
                         AuthenticationFailed = (context) =>
                         {
                             return Task.FromResult(0);
                         },
                         SecurityTokenValidated = (context) =>
                         {
                             string name = context.AuthenticationTicket.Identity.FindFirst("preferred_username").Value;
                             context.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Name, name, string.Empty));
                             return Task.FromResult(0);
                         }
                     }
                 });
        }
    }
    public static class SsoOauthAuthenticationExtensions
    {
        public static IAppBuilder UseSsoOauthAuthentication(this IAppBuilder app, List<Tenant> tenants)
        {
            SsoOauthAuthenticationOptions oauthAuthenticationOptions = new SsoOauthAuthenticationOptions(tenants);
            return app.Use(typeof(SsoOauthAuthenticationMiddleware), app, oauthAuthenticationOptions);
        }
    }
    public class SsoOauthAuthenticationMiddleware : AuthenticationMiddleware<SsoOauthAuthenticationOptions>
    {

        public SsoOauthAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, SsoOauthAuthenticationOptions options)
            : base(next, options)
        {
            foreach(var identitySetting in options.TenantIdentitySettings)
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
        public IDictionary<string, TenantAuthenticationOptions> TenantIdentitySettings { get; set; }
        
    }
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
    public class SsoOauthAuthenticationHandler : AuthenticationHandler<SsoOauthAuthenticationOptions>
    {
        private static HttpClient client;

        /// <summary>
        /// Create new ClaimsIdentity
        /// </summary>
        /// <returns>AuthenticationTicket</returns>
        protected override async Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            string tenantAlias = TenantNameResolver.Get(HttpContext.Current);

            var keyValues = new Dictionary<string, string>();
            var identity = Context.Authentication.User.Identity as ClaimsIdentity;

            keyValues.Add("grant_type", "authorization_code");
            keyValues.Add("code", Request.Query["code"]);
            keyValues.Add("redirect_uri", Options.TenantIdentitySettings[tenantAlias].RedirectUri);
            if (identity.HasClaim(c => c.Type == "refresh_token"))
            {
                string refreshToken = identity.FindFirst(x => x.Type.Equals("refresh_token")).Value;
                var refreshExpiresIn = DateTime.Parse(identity.FindFirst(x => x.Type.Equals("refresh_expires_in")).Value);
                if (DateTime.Now.Subtract(TimeSpan.FromMinutes(Constants.Buffer)) < refreshExpiresIn)
                {
                    keyValues.Add("grant_type", "refresh_token");
                    keyValues.Add("refresh_token", refreshToken);
                }
            }

            Token token = await AuthenticateAsync(keyValues, tenantAlias);
            var userInfo = GetUserInfo(token.access_token);

            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userInfo.sub));
            claims.Add(new Claim(ClaimTypes.Name, userInfo.name));
            claims.Add(new Claim(ClaimTypes.Email, userInfo.email));
            claims.Add(new Claim("tenant_alias", Options.TenantIdentitySettings[tenantAlias].TenantAlias));
            claims.Add(new Claim("locale", userInfo.locale));
            claims.Add(new Claim("id_token", token.id_token));
            claims.Add(new Claim("access_token", token.access_token));
            claims.Add(new Claim("access_expires_in", DateTime.Now.Add(TimeSpan.FromSeconds(Convert.ToDouble(token.expires_in))).ToString()));
            claims.Add(new Claim("refresh_token", token.refresh_token));
            claims.Add(new Claim("refresh_expires_in", DateTime.Now.Add(TimeSpan.FromSeconds(Convert.ToDouble(token.refresh_expires_in))).ToString()));

            identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);
            var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignIn(identity);
            var properties = Options.TenantIdentitySettings[tenantAlias].StateDataFormat.Unprotect(Request.Query["state"]);

            return await Task.FromResult(new AuthenticationTicket(identity, properties));
        }

        /// <summary>
        /// Called for every request to handle Response. 
        /// 401 will trigger Autorization Code Grant call and will redirect to 
        /// </summary>
        /// <returns></returns>
        protected override Task ApplyResponseChallengeAsync()
        {
            string tenantAlias = TenantNameResolver.Get(HttpContext.Current);
            if (Response.StatusCode == 401)
            {                
                var challenge = Helper.LookupChallenge(Options.AuthenticationType, Options.AuthenticationMode);

                // Only react to 401 if there is an authentication challenge for the authentication 
                // type of this handler.
                if (challenge != null)
                {
                    var state = challenge.Properties;

                    if (string.IsNullOrEmpty(state.RedirectUri))
                    {
                        state.RedirectUri = Request.Uri.AbsoluteUri;
                    }

                    var stateString = Options.TenantIdentitySettings[tenantAlias].StateDataFormat.Protect(state);
                    // save state in application
                    HttpContext.Current.Application[stateString] = stateString;

                    Response.Redirect(
                        WebUtilities.AddQueryString(
                            Options.TenantIdentitySettings[tenantAlias].WsdlConfig.authorization_endpoint,
                            new Dictionary<string, string>{
                                { "client_id", Options.TenantIdentitySettings[tenantAlias].ClientId },
                                { "scope", string.Join(" ",Options.TenantIdentitySettings[tenantAlias].WsdlConfig.scopes_supported) },
                                { "response_type", "code" },
                                { "state", stateString },
                                { "redirect_uri", Options.TenantIdentitySettings[tenantAlias].RedirectUri }
                            })
                    );
                }
            }
            else if (Request.Uri.AbsoluteUri.ToLower().StartsWith(Options.TenantIdentitySettings[tenantAlias].LogoutUri.ToLower()))
            {
                var claimsIdentity = HttpContext.Current.GetOwinContext().Authentication.User.Identity as System.Security.Claims.ClaimsIdentity;
                var id_token = claimsIdentity.FindFirst("id_token");

                Context.Authentication.SignOut(Options.AuthenticationType);
                IDictionary<string, string> queryString = new Dictionary<string, string>();
                queryString.Add("id_token_hint", id_token.Value);
                queryString.Add("post_logout_redirect_uri", Options.TenantIdentitySettings[tenantAlias].PostLogoutRedirectUri);
                Response.Redirect(WebUtilities.AddQueryString(Options.TenantIdentitySettings[tenantAlias].WsdlConfig.end_session_endpoint, queryString));
            }

            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Handle response from oauth server
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> InvokeAsync()
        {
            var state = Request.Query.Get("state");
            var stateOk = HttpContext.Current.Application.Get(state);
            string tenantAlias = TenantNameResolver.Get(HttpContext.Current);
            // Handle response from oauth server
            if (!string.IsNullOrEmpty(Options.TenantIdentitySettings[tenantAlias].RedirectUri) && Request.Uri.AbsoluteUri.ToLower().StartsWith(Options.TenantIdentitySettings[tenantAlias].RedirectUri.ToLower()) && stateOk != null)
            {
                var ticket = await AuthenticateAsync();

                if (ticket != null)
                {
                    Context.Authentication.SignIn(ticket.Properties, ticket.Identity);

                    //remove saved state value
                    HttpContext.Current.Application.Remove(state);

                    Response.Redirect(ticket.Properties.RedirectUri);

                    // Prevent further processing by the owin pipeline.
                    return true;
                }
            }
            // Let the rest of the pipeline run.
            return false;
        }

        /// <summary>
        /// Exchanges authorization code for access token
        /// </summary>
        /// <returns>Authentication Code Grant token</returns>
        private async Task<Token> AuthenticateAsync(Dictionary<string, string> keyValues, string tenantAlias)
        {
            return await Task.Factory.StartNew<Token>(() =>
          {
              var bodyContent = new FormUrlEncodedContent(keyValues);

              var keySecretEncode = ASCIIEncoding.ASCII.GetBytes($"{Options.TenantIdentitySettings[tenantAlias].ClientId}:{Options.TenantIdentitySettings[tenantAlias].ClientSecret}");
              var base64KeySecretEncode = Convert.ToBase64String(keySecretEncode);

              client = new HttpClient();
              var request = new HttpRequestMessage(HttpMethod.Post, Options.TenantIdentitySettings[tenantAlias].WsdlConfig.token_endpoint);
              request.Content = bodyContent;
              request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
              request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64KeySecretEncode);

              var response = client.SendAsync(request).Result;
              var result = response.Content.ReadAsStringAsync().Result;
              var serializer = new JavaScriptSerializer();
              Token token = (Token)serializer.Deserialize(result, typeof(Token));
              return token;
          });

        }

        /// <summary>
        /// Retreive user account info from Issuer
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns>UserInfo</returns>
        private UserInfo GetUserInfo(string access_token)
        {
            using (HttpClient client = new HttpClient())
            {
                string tenantAlias = TenantNameResolver.Get(HttpContext.Current);
                var request = new HttpRequestMessage(HttpMethod.Get, Options.TenantIdentitySettings[tenantAlias].WsdlConfig.userinfo_endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
                var response = client.SendAsync(request).Result;
                var result = response.Content.ReadAsStringAsync().Result;
                var serializer = new JavaScriptSerializer();
                UserInfo userInfo = (UserInfo)serializer.Deserialize(result, typeof(UserInfo));
                return userInfo;
            }
        }
    }

    /// <summary>
    /// Custom class to triger 401.  
    /// </summary>
    public class SsoOauthChallengeResult : HttpUnauthorizedResult
    {
        public string LoginProvider { get; set; }
        public string RedirectUri { get; set; }

        public SsoOauthChallengeResult(string redirectUri = "/")
        {
            LoginProvider = Constants.LoginProvider;
            RedirectUri = redirectUri;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }

    }
    public class TenantNameResolver
    {
        public static string Get(HttpContext context)
        {
            if (context.Request.RawUrl.ToCharArray().Count(s=>s=='/')>1)
            {
                return context.Request.RawUrl.Split('/')[1];
            }
            else
            {
                return Constants.DEFAULT_TENANT_ALIAS;
            }
        }
    }
    public class WsdlConfigurationProvider
    {
        public static async Task<WsdlConfig> GetAsync(string isserUrl)
        {
            return await Task.Factory.StartNew(() =>
            {
                using (var client = new HttpClient())
                {
                    string url = string.Format(@"{0}/.well-known/openid-configuration", isserUrl);
                    var request = new HttpRequestMessage(HttpMethod.Get, url);
                    var response = client.SendAsync(request).Result;
                    var result = response.Content.ReadAsStringAsync().Result;
                    var serializer = new JavaScriptSerializer();
                    WsdlConfig config = (WsdlConfig)serializer.Deserialize(result, typeof(WsdlConfig));
                    return config;
                }

            });
        }
    }
    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string id_token { get; set; }
        public string refresh_token { get; set; }
        public int refresh_expires_in { get; set; }
    }
    public class WsdlConfig
    {
        public string issuer { get; set; }
        public string authorization_endpoint { get; set; }
        public string token_endpoint { get; set; }
        public string userinfo_endpoint { get; set; }
        public string registration_endpoint { get; set; }
        public string jwks_uri { get; set; }
        public List<string> response_types_supported { get; set; }
        public List<string> response_modes_supported { get; set; }
        public List<string> grant_types_supported { get; set; }
        public List<string> subject_types_supported { get; set; }
        public List<string> id_token_signing_alg_values_supported { get; set; }
        public List<string> scopes_supported { get; set; }
        public List<string> token_endpoint_auth_methods_supported { get; set; }
        public List<string> claims_supported { get; set; }
        public List<string> code_challenge_methods_supported { get; set; }
        public string introspection_endpoint { get; set; }
        public List<string> introspection_endpoint_auth_methods_supported { get; set; }
        public string revocation_endpoint { get; set; }
        public List<string> revocation_endpoint_auth_methods_supported { get; set; }
        public string end_session_endpoint { get; set; }
        public bool request_parameter_supported { get; set; }
        public List<string> request_object_signing_alg_values_supported { get; set; }
        public string device_authorization_endpoint { get; set; }
        public List<string> dpop_signing_alg_values_supported { get; set; }
    }
    public class Address
    {
        public string street_address { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string postal_code { get; set; }
        public string country { get; set; }
    }

    public class UserInfo
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string nickname { get; set; }
        public string given_name { get; set; }
        public string middle_name { get; set; }
        public string family_name { get; set; }
        public string profile { get; set; }
        public string zoneinfo { get; set; }
        public string locale { get; set; }
        public int updated_at { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public Address address { get; set; }
        public string phone_number { get; set; }
    }
}