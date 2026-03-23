using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using SharedInfrastructure.Models;

namespace SharedInfrastructure.Services
{
    public class MicrosoftGraphService
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly HttpClient _httpClient;
        private readonly string _tenantId;

        public MicrosoftGraphService(ITokenAcquisition tokenAcquisition, HttpClient httpClient, IConfiguration configuration)
        {
            _tokenAcquisition = tokenAcquisition;
            _httpClient = httpClient;
            _tenantId = configuration["AzureAd:TenantId"];
        }

        public async Task<string> CreateGuestUserAsync(string email, string inviteRedirectUrl)
        {
            // Use application token (client credentials flow) for Graph API
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync("https://graph.microsoft.com/.default");

            var requestBody = new
            {
                invitedUserEmailAddress = email,
                inviteRedirectUrl = inviteRedirectUrl,
                sendInvitationMessage = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsync("https://graph.microsoft.com/v1.0/invitations", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to create guest user: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var invitationResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

            // Extract the invited user's ID from the response
            if (invitationResponse.TryGetProperty("invitedUser", out var invitedUser) &&
                invitedUser.TryGetProperty("id", out var userId))
            {
                return userId.GetString();
            }

            throw new Exception("Failed to extract invited user ID from Graph API response");
        }

        public async Task<List<GraphUser>> GetAllUsersAsync()
        {
            var token = await _tokenAcquisition.GetAccessTokenForAppAsync("https://graph.microsoft.com/.default");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestUrl = "https://graph.microsoft.com/v1.0/users?$select=id,displayName,mail,userType";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch users from Graph: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var root = JsonSerializer.Deserialize<JsonElement>(content);

            var users = new List<GraphUser>();
            if (root.TryGetProperty("value", out var items) && items.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in items.EnumerateArray())
                {
                    users.Add(new GraphUser
                    {
                        Id = item.GetProperty("id").GetString() ?? string.Empty,
                        DisplayName = item.GetProperty("displayName").GetString() ?? string.Empty,
                        Mail = item.TryGetProperty("mail", out var mail) ? mail.GetString() ?? string.Empty : string.Empty,
                        UserType = item.TryGetProperty("userType", out var userType) ? userType.GetString() ?? string.Empty : string.Empty
                    });
                }
            }

            return users;
        }

        public async Task<string> GetCurrentUserTypeAsync()
        {
            var token = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { "https://graph.microsoft.com/User.Read" });
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me?$select=userType");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Failed to fetch current user from Graph: {response.StatusCode} - {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<JsonElement>(content);

            if (user.TryGetProperty("userType", out var userType))
            {
                return userType.GetString() ?? "Member"; // Default to Member if not present
            }

            return "Member"; // Default
        }
    }
}