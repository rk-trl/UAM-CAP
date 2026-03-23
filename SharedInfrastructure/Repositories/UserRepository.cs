using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using SharedInfrastructure.Models;
using System.Data;

namespace SharedInfrastructure.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        public UserRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<User>("SELECT * FROM Users");
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email });
        }
        public async Task AddAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Users (Id, Email, DisplayName, Role, Status, AzureObjectId, TenantId, ApplicationId, CreatedDate) VALUES (@Id, @Email, @DisplayName, @Role, @Status, @AzureObjectId, @TenantId, @ApplicationId, @CreatedDate)";
            await connection.ExecuteAsync(sql, user);
        }
        public async Task UpdateAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Users SET DisplayName=@DisplayName, Role=@Role, Status=@Status, AzureObjectId=@AzureObjectId WHERE Id=@Id";
            await connection.ExecuteAsync(sql, user);
        }
    }
}
