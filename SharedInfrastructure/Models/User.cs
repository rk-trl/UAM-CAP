using System;

namespace SharedInfrastructure.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public string AzureObjectId { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class GraphUser
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string UserType { get; set; }
    }
}
