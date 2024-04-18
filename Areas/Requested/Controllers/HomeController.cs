using CCD_Attendance.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace CCD_Attendance.Areas.Requested.Controllers
{

    [Area("Requested")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> UnauthorizedIndex()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else if (roles.Contains("Employee"))
                {
                    return RedirectToAction("Index", "Home", new { Area = "Employee" });
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Index", "Home", new { Area = "User" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { Area = "Requested" });
                }
            }

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
