using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;
using System.Threading.Tasks;
using CapPortal.Services;
using CapPortal.Repositories;
using CapPortal.Models;
using System;

namespace CapPortal.Controllers
{
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private readonly InvitationValidationService _invitationValidationService;
        private readonly UserProvisioningService _userProvisioningService;
        private readonly IInvitationRepository _invitationRepository;

        public RegisterController(
            InvitationValidationService invitationValidationService,
            UserProvisioningService userProvisioningService,
            IInvitationRepository invitationRepository)
        {
            _invitationValidationService = invitationValidationService;
            _userProvisioningService = userProvisioningService;
            _invitationRepository = invitationRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string token)
        {
            try
            {
                var invitation = await _invitationValidationService.ValidateTokenAsync(token);
                HttpContext.Session.SetString("InvitationToken", token);
                HttpContext.Session.SetString("InvitationEmail", invitation.Email);
                HttpContext.Session.SetString("Application", invitation.Application);
                HttpContext.Session.SetString("Role", invitation.Role);

                // Redirect to Entra authentication
                return Challenge(new AuthenticationProperties { RedirectUri = "/register/callback" }, OpenIdConnectDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            try
            {
                // Get invitation context from session
                var token = HttpContext.Session.GetString("InvitationToken");
                var invitationEmail = HttpContext.Session.GetString("InvitationEmail");
                var application = HttpContext.Session.GetString("Application");
                var role = HttpContext.Session.GetString("Role");

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(invitationEmail))
                {
                    return View("Error", "Invalid session state. Please try the invitation link again.");
                }


                // Log all claims for debugging
                var allClaims = string.Join("; ", User.Claims.Select(c => $"{c.Type}: {c.Value}"));
                System.Diagnostics.Debug.WriteLine($"[RegisterController] User Claims: {allClaims}");

                // Try to get email from multiple possible claim types
                var email = User.FindFirstValue(ClaimTypes.Email)
                    ?? User.FindFirstValue("preferred_username")
                    ?? User.FindFirstValue("email");
                var name = User.FindFirstValue("name") ?? User.FindFirstValue(ClaimTypes.Name);
                var oid = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");
                var tid = User.FindFirstValue("http://schemas.microsoft.com/identity/claims/tenantid");

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(tid))
                {
                    var debugClaims = string.Join("<br>", User.Claims.Select(c => $"{c.Type}: {c.Value}"));
                    return View("Error", $"Unable to extract required user information from authentication.<br><br>Claims:<br>{debugClaims}");
                }

                // Verify email matches invitation
                if (!string.Equals(email, invitationEmail, StringComparison.OrdinalIgnoreCase))
                {
                    return View("Error", "This invitation was issued for a different email address.");
                }

                // Validate invitation again (in case it was used while user was authenticating)
                var invitation = await _invitationValidationService.ValidateTokenAsync(token);

                // Provision user in CAP
                var user = await _userProvisioningService.ProvisionUserAsync(email, name, oid, tid, role, application);

                // Mark invitation as completed
                await _invitationRepository.UpdateStatusAsync(invitation.InvitationId, "Completed", oid);

                // Clear session
                HttpContext.Session.Remove("InvitationToken");
                HttpContext.Session.Remove("InvitationEmail");
                HttpContext.Session.Remove("Application");
                HttpContext.Session.Remove("Role");

                // Redirect to dashboard or success page (Razor Pages)
                return Redirect("/Index");
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }
    }
}