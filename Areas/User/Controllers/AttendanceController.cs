using System.Text;
using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace CCD_Attendance.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "User")]
    public class AttendanceController : Controller
    {
        private readonly ccdDBContext _dbContext;

        public AttendanceController(ccdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var courses = await _dbContext.Courses
                .Where(c => c.UserId == userId && _dbContext.CourseStudents.Any(cs => cs.CourseId == c.CourseId))
                .OrderByDescending(c => c.CourseYear)
                .ThenBy(c => c.CourseSemester)
                .ThenBy(c => c.CourseName)
                .ThenBy(c => c.CRN)
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> CourseEvents(int courseId)
        {
            // Fetch the set of event IDs that have attendance records for the given course
            var eventIdsWithAttendance = await _dbContext.Attendances
                .Where(a => _dbContext.CourseStudents.Any(cs => cs.CourseId == courseId && cs.StudentId == a.StudentId))
                .Select(a => a.EventId)
                .Distinct()
                .ToListAsync();

            // Now fetch the events that are approved and have attendance data
            var events = await _dbContext.Events
                .Where(e => e.ApprovalStatus && eventIdsWithAttendance.Contains(e.EventId))
                .OrderByDescending(e => e.EventDate)
                .ToListAsync();

            ViewBag.CourseId = courseId;
            return View(events);
        }



        public async Task<IActionResult> ViewAttendance(int courseId, int eventId)
        {
            var students = await _dbContext.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .Select(cs => cs.Student)
                .ToListAsync();

            var attendingStudents = students
                .Where(s => _dbContext.Attendances.Any(a => a.EventId == eventId && a.StudentId == s.StudentId))
                .ToList();

            var model = new AttendanceDetailViewModel
            {
                Course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId),
                Event = await _dbContext.Events.FirstOrDefaultAsync(e => e.EventId == eventId),
                Students = attendingStudents
            };

            return View(model);
        }

        public async Task<IActionResult> DownloadAttendance(int courseId, int eventId)
        {
            var students = await _dbContext.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .Select(cs => cs.Student)
                .ToListAsync();

            var attendingStudents = students
                .Where(s => _dbContext.Attendances.Any(a => a.EventId == eventId && a.StudentId == s.StudentId))
                .ToList();

            var builder = new StringBuilder();
            builder.AppendLine("StudentId,Username,FirstName,LastName");
            foreach (var student in attendingStudents)
            {
                builder.AppendLine($"{student.StudentId},{student.Username},{student.FirstName},{student.LastName}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"Attendance-{eventId}-{DateTime.UtcNow:yyyyMMdd}.csv");
        }
    }
}
