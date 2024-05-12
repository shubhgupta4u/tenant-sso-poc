namespace tenant_sso_poc.models
{
    public class Tenant
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Locale { get; set; }
        public string IdentityId { get; set; }   
    }
}
