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
                .OrderByDescending(c => c.CourseYear) // Sort by Year descending
                .ThenBy(c => c.CourseSemester)         // Then by Semester alphabetically
                .ThenBy(c => c.CourseName)             // Then by Course Name alphabetically
                .ThenBy(c => c.CRN)                    // Then by CRN
                .ToList();

            return View(courses);
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Course course = new Course();
            course.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Course course)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            course.UserId = userId;
            course.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);

            if (_dbContext.Courses.Any(c => c.CRN == course.CRN))
            {
                ModelState.AddModelError("CRN", "A course with this CRN already exists.");
                return View(course); // Return here to show the error in the view
            }

            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses.FirstOrDefault(c => c.CRN == id && c.UserId == userId);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Course courseModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses.FirstOrDefault(c => c.CRN == id && c.UserId == userId);
            if (course == null)
            {
                return NotFound();
            }

            if (_dbContext.Courses.Any(c => c.CRN == courseModel.CRN && c.CRN != id))
            {
                ModelState.AddModelError("CRN", "Another course with this CRN already exists.");
                return View(courseModel); // Return here to show the error in the view
            }

            course.CourseName = courseModel.CourseName;
            course.CourseSection = courseModel.CourseSection;
            course.CourseSemester = courseModel.CourseSemester;
            course.CourseYear = courseModel.CourseYear;

            _dbContext.Update(course);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = _dbContext.Courses.FirstOrDefault(c => c.CRN == id && c.UserId == userId);
            if (course == null)
            {
                return NotFound();
            }

            _dbContext.Courses.Remove(course);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
