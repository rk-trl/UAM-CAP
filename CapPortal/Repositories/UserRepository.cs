using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using CapPortal.Models;

namespace CapPortal.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        public UserRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Users WHERE Email = @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }
        public async Task AddAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Users (Id, Email, Name, EntraObjectId, TenantId, Role, ApplicationId, CreatedDate) VALUES (@Id, @Email, @Name, @EntraObjectId, @TenantId, @Role, @ApplicationId, @CreatedDate)";
            await connection.ExecuteAsync(sql, user);
        }
    }
}