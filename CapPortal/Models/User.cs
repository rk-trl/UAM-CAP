using System;

namespace CapPortal.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string EntraObjectId { get; set; }
        public string TenantId { get; set; }
        public string Role { get; set; }
        public string ApplicationId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}