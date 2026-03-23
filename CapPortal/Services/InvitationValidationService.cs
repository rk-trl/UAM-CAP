using System;
using System.Threading.Tasks;
using CapPortal.Repositories;
using CapPortal.Models;

namespace CapPortal.Services
{
    public class InvitationValidationService
    {
        private readonly IInvitationRepository _invitationRepository;
        public InvitationValidationService(IInvitationRepository invitationRepository)
        {
            _invitationRepository = invitationRepository;
        }
        public async Task<Invitation> ValidateTokenAsync(string token)
        {
            var invitation = await _invitationRepository.GetByTokenAsync(token);
            if (invitation == null)
                throw new Exception("Invalid invitation token.");
            // If you want to add expiry logic, add an ExpiryDate field to the model and table
            if (invitation.Status != "Pending")
                throw new Exception("Invitation already used or invalid.");
            return invitation;
        }
    }
}