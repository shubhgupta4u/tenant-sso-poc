using Microsoft.VisualStudio.TestTools.UnitTesting;
using tenant_sso_poc.repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;

namespace tenant_sso_poc.repository.Tests
{
    [TestClass()]
    public class TenantSeedTests
    {        
        [TestMethod()]
        public async Task StartAsyncTest()
        {
            ITenantRepository tenantRepository = new TenantRepository();
            TenantSeed tenantSeed = new TenantSeed(tenantRepository);
            tenantSeed.StartAsync().Wait();

            var result = await tenantRepository.GetAllAsync();
            Assert.IsTrue(result.Count() == 2);

            var tenant = new Tenant() { Id = Constants.GOT_TENANTID, Alias = "ONB", Name = "Onb", Locale = "en-US", IdentityId = Constants.GOT_AUTHORITYID };
            await tenantRepository.UpdateAsync(tenant);
        }
    }
}