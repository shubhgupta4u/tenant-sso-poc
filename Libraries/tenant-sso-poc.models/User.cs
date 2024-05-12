namespace tenant_sso_poc.models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SSN { get; set; }
        public string TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string[] RoleIds { get; set; }
        public Role[] Roles { get; set; }
    }
}
