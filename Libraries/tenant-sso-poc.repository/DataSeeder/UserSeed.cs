using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository.DataSeeder
{
    public class UserSeed : IDataSeeder<User>
    {
        IUserRepository _userRepository;
        public UserSeed(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task StartAsync()
        {
            List<User> tenants = await GetDummyEntitiesAsync();
            Parallel.ForEach(tenants, async (item) =>
            {
                await _userRepository.CreateAsync(item);
            });
        }
        public async Task<List<User>> GetDummyEntitiesAsync()
        {
            return await Task.Factory.StartNew<List<User>>(() =>
            {
                List<User> users = new List<User>();
                users.Add(new User() { Id = Guid.NewGuid().ToString(), UserName= "talisa@got.us", Email ="talisa@got.us",FirstName="Talisa",TenantId = Constants.GOT_TENANTID });
                users.Add(new User() { Id = Guid.NewGuid().ToString(), UserName = "stark@got.us", Email = "stark@got.us", FirstName = "Stark", TenantId = Constants.GOT_TENANTID });
                users.Add(new User() { Id = Guid.NewGuid().ToString(), UserName = "talisa@mordor.us", Email = "talisa@mordor.us", FirstName = "Talisa", TenantId = Constants.MORDOR_TENANTID });
                users.Add(new User() { Id = Guid.NewGuid().ToString(), UserName = "stark@mordor.us", Email = "stark@mordor.us", FirstName = "Stark", TenantId = Constants.MORDOR_TENANTID });
                return users;
            });

        }
    }
}
