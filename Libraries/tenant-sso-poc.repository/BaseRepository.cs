using Dapper;
using System.Threading.Tasks;
using tenant_sso_poc.data;

namespace tenant_sso_poc.repository
{
    public class BaseRepository<T>
    {
        static BaseRepository()
        {
            Init().Wait();
        }
        private static async Task Init()
        {
            // create database tables if they don't exist
            using (var connection = DataContext.Instance.CreateConnection())
            {
                connection.Open();
                var sql = "";
                switch (typeof(T).Name)
                {
                    case "User":
                        sql = Constants.CREATE_USERS_QUERY;
                        break;
                    case "Tenant":
                        sql = Constants.CREATE_TENANTS_QUERY;
                        break;
                    case "IdentitySetting":
                        sql = Constants.CREATE_IDENTITYSETTING_QUERY;
                        break;
                }
                if (!string.IsNullOrEmpty(sql))
                {
                    await connection.ExecuteAsync(sql);
                }
                connection.Close();
            }
        }
    }
}
