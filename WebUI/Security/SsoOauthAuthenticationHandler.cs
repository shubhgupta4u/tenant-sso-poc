using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using tenant_sso_poc.WebUI.Helpers;
using tenant_sso_poc.WebUI.Models.Oauth;

namespace tenant_sso_poc.WebUI.Security
{
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
}