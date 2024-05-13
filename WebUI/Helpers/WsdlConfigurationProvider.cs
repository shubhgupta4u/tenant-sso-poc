using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using tenant_sso_poc.WebUI.Models.Oauth;

namespace tenant_sso_poc.WebUI.Helpers
{
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
}