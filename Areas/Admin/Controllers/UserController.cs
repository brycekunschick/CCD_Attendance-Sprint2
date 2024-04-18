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
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ccdDBContext _context;

        public UserController(UserManager<IdentityUser> userManager, ccdDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch all users
            var allUsers = await _userManager.Users.ToListAsync();

            // Prepare lists to hold user view models
            var requestedUsers = new List<UserViewModel>();
            var regularUsers = new List<UserViewModel>();

            // Iterate through all users to classify them based on their roles
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var applicationUser = await _context.ApplicationUsers.FindAsync(user.Id);

                if (applicationUser != null)
                {
                    var userViewModel = new UserViewModel
                    {
                        Id = applicationUser.Id,
                        Email = applicationUser.Email,
                        FirstName = applicationUser.FirstName,
                        LastName = applicationUser.LastName
                    };

                    if (roles.Contains("Requested"))
                    {
                        requestedUsers.Add(userViewModel);
                    }
                    else if (roles.Contains("User"))
                    {
                        regularUsers.Add(userViewModel);
                    }
                }
            }

            var model = new UsersViewModel
            {
                RequestedUsers = requestedUsers,
                RegularUsers = regularUsers
            };

            return View(model);
        }


        public async Task<IActionResult> Approve(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.RemoveFromRoleAsync(user, "Requested");
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deny(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Promote(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                await _userManager.RemoveFromRoleAsync(user, "User");
            }
            return RedirectToAction(nameof(Index));
        }




    }
}
