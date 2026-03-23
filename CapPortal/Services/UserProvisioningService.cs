using System;
using System.Threading.Tasks;
using CapPortal.Repositories;
using CapPortal.Models;

namespace CapPortal.Services
{
    public class UserProvisioningService
    {
        private readonly IUserRepository _userRepository;
        public UserProvisioningService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> ProvisionUserAsync(string email, string name, string entraObjectId, string tenantId, string role, string applicationId)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
                return existingUser;
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                EntraObjectId = entraObjectId,
                TenantId = tenantId,
                Role = role,
                ApplicationId = applicationId,
                CreatedDate = DateTime.UtcNow
            };
            await _userRepository.AddAsync(user);
            return user;
        }
    }
}