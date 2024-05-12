using System.Threading.Tasks;
using tenant_sso_poc.models;

namespace tenant_sso_poc.repository.Interfaces
{
    public interface ITenantRepository : IBaseRespository<Tenant>
    {
        Task<Tenant> GetByAlias(string alias);
    }
}
