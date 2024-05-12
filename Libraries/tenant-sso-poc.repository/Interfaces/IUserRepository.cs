using System.Threading.Tasks;
using tenant_sso_poc.models;

namespace tenant_sso_poc.repository.Interfaces
{
    public interface IUserRepository : IBaseRespository<User>
    {
        Task<User> GetByEmail(string email);
    }
}
