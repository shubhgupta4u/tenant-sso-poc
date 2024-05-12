using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using tenant_sso_poc.data;
using tenant_sso_poc.models;
using tenant_sso_poc.repository.Interfaces;

namespace tenant_sso_poc.repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository()
        {
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Users
            ";
                return await connection.QueryAsync<User>(sql);
            }
        }

        public async Task<User> GetByIdAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Users
           WHERE Id = @id
            ";
                return await connection.QuerySingleOrDefaultAsync<User>(sql, new { id });
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                SELECT* FROM Users
           WHERE Email = @email
            ";
                return await connection.QuerySingleOrDefaultAsync<User>(sql, new { email });
            }
        }

        public async Task CreateAsync(User user)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                INSERT INTO Users(Id, UserName, FirstName, LastName, Email, SSN,TenantId)
                VALUES(@Id, @UserName, @FirstName, @LastName, @Email, @SSN,@TenantId)
            ";
                await connection.ExecuteAsync(sql, user);
            }
        }

        public async Task UpdateAsync(User user)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                UPDATE Users
                SET UserName = @UserName,
                FirstName = @FirstName,
                LastName = @LastName,
                Email = @Email,
                SSN = @SSN,
                TenantId = @TenantId
                WHERE Id = @Id
            ";
                await connection.ExecuteAsync(sql, user);
            }
        }

        public async Task DeleteAsync(string id)
        {
            using (var connection = DataContext.Instance.CreateConnection())
            {
                var sql = @"
                DELETE FROM Users
            WHERE Id = @id
            ";
                await connection.ExecuteAsync(sql, new { id });
            }
        }
    }
}
