using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CapPortal.Repositories;
using System.Threading.Tasks;

namespace CapPortal.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IActionResult> PostLogin()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("NotRegistered", "Account");
            }

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                // User not registered, redirect to not registered page
                return RedirectToAction("NotRegistered", "Account");
            }
            // User exists, proceed to dashboard
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotRegistered()
        {
            return View();
        }
    }
}
