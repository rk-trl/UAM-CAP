using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedInfrastructure.Repositories;
using SharedInfrastructure.Models;
using SharedInfrastructure.Services;
using System;
using System.Threading.Tasks;

namespace UamPortal.Pages
{
    [Authorize(Roles = "Admin")]
    public class InviteModel : PageModel
    {
        private readonly InvitationRepository _invitationRepository;
        private readonly UserRepository _userRepository;
        private readonly MicrosoftGraphService _graphService;
        private readonly SendGridEmailService _emailService;
        public string InvitationUrl { get; set; } = string.Empty;

        public InviteModel(
            InvitationRepository invitationRepository,
            UserRepository userRepository,
            MicrosoftGraphService graphService,
            SendGridEmailService emailService)
        {
            _invitationRepository = invitationRepository;
            _userRepository = userRepository;
            _graphService = graphService;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string Email, string ApplicationId, string RoleId)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userRepository.GetByEmailAsync(Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "User already exists with this email address.");
                    return Page();
                }

                // Generate invitation token
                var token = Guid.NewGuid().ToString();
                var invitationUrl = $"https://localhost:5002/register?token={token}";

                // Create guest user in Entra ID via Microsoft Graph API
                var entraUserId = await _graphService.CreateGuestUserAsync(Email, invitationUrl);

                // Create invitation record in database
                var invitation = new Invitation
                {
                    InvitationId = Guid.NewGuid(),
                    Email = Email,
                    Application = ApplicationId,
                    Role = RoleId,
                    Token = token,
                    Status = "Pending",
                    EntraObjectId = entraUserId
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

                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to send invitation: {ex.Message}");
                return Page();
            }
        }
    }
}
