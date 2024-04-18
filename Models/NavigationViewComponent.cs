using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CCD_Attendance.Models;

namespace CCD_Attendance.Models
{
    public class NavigationViewComponent : ViewComponent
    {
        private readonly UserManager<IdentityUser> _userManager;

        public NavigationViewComponent(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string area = "";

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Admin"))
                {
                    area = "Admin";
                }
                else if (roles.Contains("Employee"))
                {
                    area = "Employee";
                }
                else if (roles.Contains("User"))
                {
                    area = "User";
                }
                else
                {
                    area = "Requested";
                }
            }

            return View("Default", area);
        }
    }
}
