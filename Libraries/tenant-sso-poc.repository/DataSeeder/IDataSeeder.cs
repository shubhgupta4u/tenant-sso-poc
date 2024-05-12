using System.Collections.Generic;
using System.Threading.Tasks;

namespace tenant_sso_poc.repository.DataSeeder
{
    public interface IDataSeeder<T>
    {
        Task StartAsync();
        Task<List<T>> GetDummyEntitiesAsync();
    }
}
