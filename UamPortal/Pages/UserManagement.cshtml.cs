using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedInfrastructure.Repositories;
using SharedInfrastructure.Models;
using SharedInfrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UamPortal.Pages
{
    [Authorize]
    public class UserManagementModel : PageModel
    {
        private readonly UserRepository _userRepository;
        private readonly InvitationRepository _invitationRepository;
        private readonly MicrosoftGraphService _graphService;
        private readonly SendGridEmailService _emailService;
        public List<User> Users { get; set; } = new List<User>();
        public List<GraphUser> InternalUsers { get; set; } = new List<GraphUser>();
        public List<GraphUser> ExternalUsers { get; set; } = new List<GraphUser>();
        public List<Invitation> ExternalInvitations { get; set; } = new List<Invitation>();
        public string InvitationUrl { get; set; } = string.Empty;

        public UserManagementModel(
            UserRepository userRepository,
            InvitationRepository invitationRepository,
            MicrosoftGraphService graphService,
            SendGridEmailService emailService)
        {
            _userRepository = userRepository;
            _invitationRepository = invitationRepository;
            _graphService = graphService;
            _emailService = emailService;
        }

        public async Task OnGetAsync(string email = null)
        {
            // One Graph API call to fetch all users and categorize by userType
            var graphUsers = await _graphService.GetAllUsersAsync();
            InternalUsers = graphUsers.FindAll(u => !string.Equals(u.UserType, "Guest", StringComparison.OrdinalIgnoreCase));
            ExternalUsers = graphUsers.FindAll(u => string.Equals(u.UserType, "Guest", StringComparison.OrdinalIgnoreCase));

            // Keep existing data for backwards compatibility (existing flows)
            Users = new List<User>((await _userRepository.GetAllAsync()));
            ExternalInvitations = new List<Invitation>((await _invitationRepository.GetAllAsync()));

            if (!string.IsNullOrEmpty(email))
            {
                var latestInvitation = await _invitationRepository.GetLatestByEmailAsync(email);
                if (latestInvitation != null)
                {
                    InvitationUrl = $"https://localhost:5002/register?token={latestInvitation.Token}";
                }
            }
        }

        public async Task<IActionResult> OnPostAsync(string Email, string Role)
        {
            try
            {
                var applicationValues = Request.Form["Application"];
                var application = applicationValues.Count > 0 ? string.Join(", ", applicationValues.ToArray()) : "CAP";

                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "User already exists with this email address.");
                    Users = new List<User>((await _userRepository.GetAllAsync()));
                    ExternalInvitations = new List<Invitation>((await _invitationRepository.GetAllAsync()));
                    // Show latest invitation if exists
                    var latestInvitation = await _invitationRepository.GetLatestByEmailAsync(Email);
                    if (latestInvitation != null)
                        InvitationUrl = $"https://localhost:5002/register?token={latestInvitation.Token}";
                    return Page();
                }

                // Generate invitation token
                var token = Guid.NewGuid().ToString();
                var invitationUrl = $"https://localhost:5002/register?token={token}";

                // Create guest user in Entra ID via Microsoft Graph API
                var entraObjectId = await _graphService.CreateGuestUserAsync(Email, invitationUrl);

                // Create invitation record in database
                var invitation = new Invitation
                {
                    InvitationId = Guid.NewGuid(),
                    Email = Email,
                    Token = token,
                    Application = application,
                    Role = Role,
                    Status = "Pending",
                    EntraObjectId = entraObjectId
                };

                await _invitationRepository.AddAsync(invitation);

                // Send invitation email
                var subject = "You're invited to join CAP Portal";
                var htmlContent = $@"
                    <h2>Welcome to CAP Portal</h2>
                    <p>You've been invited to join the CAP Portal. Click the link below to complete your registration:</p>
                    <p><a href='{invitationUrl}'>Complete Registration</a></p>
                    <p>This invitation will expire in 48 hours.</p>
                    <p>If you have any questions, please contact your administrator.</p>
                ";

                await _emailService.SendEmailAsync(Email, subject, htmlContent);

                InvitationUrl = invitationUrl;
                ViewData["SuccessMessage"] = "Invitation sent successfully!";
                Users = new List<User>((await _userRepository.GetAllAsync()));
                ExternalInvitations = new List<Invitation>((await _invitationRepository.GetAllAsync()));

                // Redirect to GET with email param to persist link after refresh
                return RedirectToPage(new { email = Email });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to send invitation: {ex.Message}");
                Users = new List<User>((await _userRepository.GetAllAsync()));
                ExternalInvitations = new List<Invitation>((await _invitationRepository.GetAllAsync()));
                return Page();
            }
        }
    }
}
