using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.DataSeeder;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository
{
    public class TenantSeed: IDataSeeder<Tenant>
    {
        ITenantRepository _tenantRepository;
        public TenantSeed(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task StartAsync()
        {
            List<Tenant> tenants = await GetDummyEntitiesAsync();
            Parallel.ForEach(tenants, async(item) =>
            {
                await _tenantRepository.CreateAsync(item);
            });
        }
        public async Task<List<Tenant>> GetDummyEntitiesAsync()
        {            
            return await Task.Factory.StartNew<List<Tenant>>(() =>
            {
                List<Tenant> tenants = new List<Tenant>();
                tenants.Add(new Tenant() { Id = Constants.GOT_TENANTID, Alias = "ONB", Name = "onb", Locale = "en-US", IdentityId = Constants.GOT_AUTHORITYID });
                tenants.Add(new Tenant() { Id = Constants.MORDOR_TENANTID, Alias = "MORDOR", Name = "Mordor", Locale = "ru-RU", IdentityId = Constants.MORDOR_AUTHORITYID });
                return tenants;
            });
           
        }
    }
}
