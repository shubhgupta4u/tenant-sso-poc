using Microsoft.VisualStudio.TestTools.UnitTesting;
using tenant_sso_poc.repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;


namespace tenant_sso_poc.repository.DataSeeder.Tests
{
    [TestClass()]
    public class IdentitySettingSeedTests
    {

        [TestMethod()]
        public async Task StartAsyncTest()
        {
            IIdentitySettingRepository identitySettingRepository = new IdentitySettingRespository();
            //IdentitySettingSeed settingSeed = new IdentitySettingSeed(identitySettingRepository);
            //settingSeed.StartAsync().Wait();

            var result = await identitySettingRepository.GetAllAsync();
            Assert.IsTrue(result.Count() == 2);
        }
    }
}