namespace tenant_sso_poc.models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string[] PermissionIds { get; set; }
        public Permission[] Permissions { get; set; }
    }
}
