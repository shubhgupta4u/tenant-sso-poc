using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository
{
    public class IdentitySettingRespository : BaseRepository<IdentitySetting>, IIdentitySettingRepository
    {
        public IdentitySettingRespository()
        {
        }
        public async Task<IEnumerable<IdentitySetting>> GetAllAsync()
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                                SELECT* FROM IdentitySettings
                            ";
                return await connection.QueryAsync<IdentitySetting>(sql);

            }
        }

        public async Task<IdentitySetting> GetByIdAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM IdentitySettings
           WHERE Id = @id
            ";
                return await connection.QuerySingleOrDefaultAsync<IdentitySetting>(sql, new { id });
            }
        }

        public async Task<IdentitySetting> GetByAuthority(string authority)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM IdentitySettings
           WHERE Alias = @alias
            ";
                return await connection.QuerySingleOrDefaultAsync<IdentitySetting>(sql, new { authority });
            }
        }

        public async Task CreateAsync(IdentitySetting identitySetting)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                INSERT INTO IdentitySettings(Id, ClientId, ClientSecret, AuthorityUrl, GrantType,Audience,SupportLocale)
                VALUES(@Id, @ClientId, @ClientSecret, @AuthorityUrl,@GrantType,@Audience,@SupportLocale)
            ";
                await connection.ExecuteAsync(sql, identitySetting);
            }
        }

        public async Task UpdateAsync(IdentitySetting identitySetting)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                UPDATE IdentitySettings
                SET ClientId = @ClientId,
                ClientSecret = @ClientSecret,
                AuthorityUrl = @AuthorityUrl,
                GrantType = @GrantType,
                Audience = @Audience,
                SupportLocale = @SupportLocale
                WHERE Id = @Id
                ";
                await connection.ExecuteAsync(sql, identitySetting);
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                DELETE FROM IdentitySettings
            WHERE Id = @id
            ";
                await connection.ExecuteAsync(sql, new { id });
            }
        }
    }
}
