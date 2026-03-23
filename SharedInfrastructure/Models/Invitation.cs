using System;

namespace SharedInfrastructure.Models
{
    public class Invitation
    {
        public Guid InvitationId { get; set; } // PK
        public string Email { get; set; }
        public string Token { get; set; }
        public string Application { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string EntraObjectId { get; set; }
    }
}
