using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using SharedInfrastructure.Models;
using System.Data;

namespace SharedInfrastructure.Repositories
{
    public class InvitationRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        public InvitationRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<IEnumerable<Invitation>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Invitation>("SELECT * FROM Invitations");
        }

        public async Task<Invitation> GetByTokenAsync(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Invitation>("SELECT * FROM Invitations WHERE Token = @Token", new { Token = token });
        }

        public async Task<Invitation> GetLatestByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Invitation>(
                "SELECT TOP 1 * FROM Invitations WHERE Email = @Email ORDER BY InvitationId DESC", new { Email = email });
        }

        public async Task AddAsync(Invitation invitation)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "INSERT INTO Invitations (InvitationId, Email, Token, Application, Role, Status, EntraObjectId) VALUES (@InvitationId, @Email, @Token, @Application, @Role, @Status, @EntraObjectId)";
            await connection.ExecuteAsync(sql, invitation);
        }

        public async Task UpdateStatusAsync(Guid invitationId, string status, string entraObjectId = null)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "UPDATE Invitations SET Status=@Status, EntraObjectId=@EntraObjectId WHERE InvitationId=@InvitationId";
            await connection.ExecuteAsync(sql, new { InvitationId = invitationId, Status = status, EntraObjectId = entraObjectId });
        }

        public async Task<int> GetPendingInvitationsCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Invitations WHERE Status = 'Pending'");
        }
    }
}
