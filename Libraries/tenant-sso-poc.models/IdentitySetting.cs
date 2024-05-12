using System.ComponentModel.DataAnnotations;

namespace tenant_sso_poc.models
{
    public class IdentitySetting
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
        [Required]
        public string AuthorityUrl { get; set; }
        public string GrantType { get; set; }
        public string Audience { get; set; }
        public string[] Scope { get; set; }
        [Required]
        public bool SupportLocale { get; set; }
    }
}
