using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SharedInfrastructure.Repositories;
using System.Threading.Tasks;

namespace UamPortal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;
        public AccountController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> PostLogin()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null || user.Role != "Admin" || user.Status != "Active")
            {
                // Not a valid admin user
                return RedirectToAction("AccessDenied", "Account");
            }
            // Valid admin, proceed to dashboard
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
