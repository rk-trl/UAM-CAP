using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SharedInfrastructure.Services;
using SharedInfrastructure.Repositories;

namespace UamPortal.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MicrosoftGraphService _graphService;
    private readonly InvitationRepository _invitationRepository;

    public IndexModel(ILogger<IndexModel> logger, MicrosoftGraphService graphService, InvitationRepository invitationRepository)
    {
        _logger = logger;
        _graphService = graphService;
        _invitationRepository = invitationRepository;
    }

    public int TotalUsers { get; set; }
    public int InternalUsers { get; set; }
    public int ExternalUsers { get; set; }
    public int PendingInvitations { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var users = await _graphService.GetAllUsersAsync();
            InternalUsers = users.Count(u => u.UserType == "Member");
            ExternalUsers = users.Count(u => u.UserType == "Guest");
            TotalUsers = InternalUsers + ExternalUsers;

            PendingInvitations = await _invitationRepository.GetPendingInvitationsCountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching dashboard metrics");
            // Set defaults to 0
            TotalUsers = 0;
            InternalUsers = 0;
            ExternalUsers = 0;
            PendingInvitations = 0;
        }
    }
}
