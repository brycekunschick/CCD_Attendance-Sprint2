// Areas/Admin/Controllers/EmployeeController.cs
using CCD_Attendance.Areas.Admin.Models;
using CCD_Attendance.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCD_Attendance.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ccdDBContext _context;

        public EmployeeController(UserManager<IdentityUser> userManager, ccdDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var employees = new List<UserViewModel>();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Employee"))
                {
                    var applicationUser = await _context.ApplicationUsers.FindAsync(user.Id);
                    if (applicationUser != null)
                    {
                        employees.Add(new UserViewModel
                        {
                            Id = applicationUser.Id,
                            Email = applicationUser.Email,
                            FirstName = applicationUser.FirstName,
                            LastName = applicationUser.LastName
                        });
                    }
                }
            }

            return View(employees);
        }

        public async Task<IActionResult> Demote(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.RemoveFromRoleAsync(user, "Employee");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
