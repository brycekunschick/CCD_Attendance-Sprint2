using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace CCD_Attendance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class CourseController : Controller
    {
        private readonly ccdDBContext _dbContext;

        public CourseController(ccdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = _dbContext.Courses
                .Where(c => c.UserId == userId)
                .Include(c => c.ApplicationUser)
                .ToList();

            return View(courses);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//fetches the user ID

            Course course = new Course();

            course.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);



            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);//fetches the user ID
            course.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);


            course.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Assuming Edit and Delete actions would be similar
    }
}
