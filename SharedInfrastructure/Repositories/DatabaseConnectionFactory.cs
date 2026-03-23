using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SharedInfrastructure.Repositories
{
    public class DatabaseConnectionFactory
    {
        private readonly string _connectionString;
        public DatabaseConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
