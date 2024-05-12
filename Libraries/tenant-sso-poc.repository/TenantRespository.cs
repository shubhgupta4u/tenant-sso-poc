using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository
{
    public class TenantRepository : BaseRepository<Tenant>, ITenantRepository
    {
        public TenantRepository()
        {
        }
        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Tenants
            ";
                return await connection.QueryAsync<Tenant>(sql);
            }
        }

        public async Task<Tenant> GetByIdAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Tenants
           WHERE Id = @id
            ";
                Tenant tenant = await connection.QuerySingleOrDefaultAsync<Tenant>(sql, new { id });
                return tenant;
            }
        }

        public async Task<Tenant> GetByAlias(string alias)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Tenants
           WHERE Alias = @alias
            ";
                return await connection.QuerySingleOrDefaultAsync<Tenant>(sql, new { alias });
            }
        }

        public async Task CreateAsync(Tenant tenant)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                INSERT INTO Tenants(Id, Name, Alias, Locale, IdentityId)
                VALUES(@Id, @Name, @Alias, @Locale,@IdentityId)
            ";
                await connection.ExecuteAsync(sql, tenant);
            }
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                UPDATE Tenants
                SET Name = @Name,
                Alias = @Alias,
                Locale = @Locale,
                IdentityId = @IdentityId
                WHERE Id = @Id
            ";
                await connection.ExecuteAsync(sql, tenant);
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                DELETE FROM Tenants
            WHERE Id = @id
            ";
                await connection.ExecuteAsync(sql, new { id });
            }
        }
    }
}
