using System.Collections.Generic;
using System.Threading.Tasks;

namespace tenant_sso_poc.repository.Interfaces
{
    public interface IBaseRespository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task CreateAsync(T item);
        Task UpdateAsync(T item);
        Task DeleteAsync(string id);
    }
}
