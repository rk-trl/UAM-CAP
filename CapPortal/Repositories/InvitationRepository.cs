using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Dapper;
using CapPortal.Models;

namespace CapPortal.Repositories
{
    public interface IInvitationRepository
    {
        Task<Invitation> GetByTokenAsync(string token);
        Task AddAsync(Invitation invitation);
        Task UpdateStatusAsync(Guid invitationId, string status, string entraObjectId = null);
    }

    public class InvitationRepository : IInvitationRepository
    {
        private readonly DatabaseConnectionFactory _connectionFactory;
        public InvitationRepository(DatabaseConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<Invitation> GetByTokenAsync(string token)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Invitations WHERE Token = @Token";
            return await connection.QueryFirstOrDefaultAsync<Invitation>(sql, new { Token = token });
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
    }
}