using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using tenant_sso_poc.repository.Interfaces;
using System.Linq;

namespace tenant_sso_poc.repository.DataSeeder.Tests
{
    [TestClass()]
    public class UserSeedTests
    {
        [TestMethod()]
        public async Task StartAsyncTest()
        {
            IUserRepository userRepository = new UserRepository();
            //UserSeed userSeed = new UserSeed(userRepository);
            //userSeed.StartAsync().Wait();

            var result = await userRepository.GetAllAsync();
            Assert.IsTrue(result.Count() == 4);
        }
    }
}