using System.Threading.Tasks;
using tenant_sso_poc.models;

namespace tenant_sso_poc.repository.Interfaces
{
    public interface IIdentitySettingRepository: IBaseRespository<IdentitySetting>
    {
        Task<IdentitySetting> GetByAuthority(string authority);
    }
}
