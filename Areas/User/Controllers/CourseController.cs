using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IO;

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
                .Select(course => new CourseViewModel
                {
                    CourseId = course.CourseId,
                    CRN = course.CRN,
                    CourseName = course.CourseName,
                    CourseSection = course.CourseSection,
                    CourseSemester = course.CourseSemester,
                    CourseYear = course.CourseYear,
                    HasRoster = _dbContext.CourseStudents.Any(cs => cs.CourseId == course.CourseId)
                })
                .OrderByDescending(c => c.CourseYear)
                .ThenBy(c => c.CourseSemester)
                .ThenBy(c => c.CourseName)
                .ThenBy(c => c.CRN)
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


        //Roster actions

        [HttpGet]
        public IActionResult UploadRoster(int courseId)
        {
            var model = new UploadRosterViewModel
            {
                CourseId = courseId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadRoster(UploadRosterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                return View(model);
            }

            using (var stream = new StreamReader(model.File.OpenReadStream()))
            {
                string headerLine = await stream.ReadLineAsync();
                var headers = headerLine.Split(',');
                if (headers.Length != 3 || headers[2].Trim() != "Email Address")
                {
                    ModelState.AddModelError("File", "Incorrect CSV format.");
                    return View(model);
                }

                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var data = line.Split(',');

                    var email = data[2].Trim();
                    var username = email.Substring(0, email.IndexOf('@'));

                    var student = _dbContext.Students.FirstOrDefault(s => s.Username == username);
                    if (student == null)
                    {
                        student = new Student
                        {
                            FirstName = data[0].Trim(),
                            LastName = data[1].Trim(),
                            Username = username
                        };
                        _dbContext.Students.Add(student);
                        await _dbContext.SaveChangesAsync();
                    }

                    var courseStudent = new CourseStudent
                    {
                        StudentId = student.StudentId,
                        CourseId = model.CourseId
                    };
                    _dbContext.CourseStudents.Add(courseStudent);
                }
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult UpdateRoster(int courseId)
        {
            var model = new UploadRosterViewModel
            {
                CourseId = courseId
            };
            return View("UploadRoster", model);  // Reusing the UploadRoster view for updates
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoster(UploadRosterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UploadRoster", model);
            }

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                return View("UploadRoster", model);
            }

            var students = new List<Student>();
            var courseStudents = new List<CourseStudent>();

            using (var stream = new StreamReader(model.File.OpenReadStream()))
            {
                string headerLine = await stream.ReadLineAsync();
                var headers = headerLine.Split(',');
                if (headers.Length != 3 || headers[2].Trim() != "Email Address")
                {
                    ModelState.AddModelError("File", "Incorrect CSV format.");
                    return View("UploadRoster", model);
                }

                // Remove existing course-student links before adding new ones
                var existingCourseStudents = _dbContext.CourseStudents.Where(cs => cs.CourseId == model.CourseId);
                _dbContext.CourseStudents.RemoveRange(existingCourseStudents);
                await _dbContext.SaveChangesAsync();

                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var data = line.Split(',');

                    var email = data[2].Trim();
                    var username = email.Substring(0, email.IndexOf('@'));

                    var student = _dbContext.Students.FirstOrDefault(s => s.Username == username);
                    if (student == null)
                    {
                        student = new Student
                        {
                            FirstName = data[0].Trim(),
                            LastName = data[1].Trim(),
                            Username = username
                        };
                        _dbContext.Students.Add(student);
                        await _dbContext.SaveChangesAsync();
                    }

                    courseStudents.Add(new CourseStudent
                    {
                        StudentId = student.StudentId,
                        CourseId = model.CourseId
                    });
                }
            }

            _dbContext.CourseStudents.AddRange(courseStudents);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRoster(int courseId)
        {
            var courseStudents = _dbContext.CourseStudents.Where(cs => cs.CourseId == courseId);
            _dbContext.CourseStudents.RemoveRange(courseStudents);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        //view a roster
        [HttpGet]
        public IActionResult ViewRoster(int courseId)
        {
            var course = _dbContext.Courses
                .Where(c => c.CourseId == courseId)
                .Include(c => c.ApplicationUser) // Include details of the course creator if needed
                .FirstOrDefault();

            if (course == null)
            {
                return NotFound();
            }

            var students = _dbContext.CourseStudents
                .Where(cs => cs.CourseId == courseId)
                .Include(cs => cs.Student)
                .Select(cs => new StudentViewModel
                {
                    StudentId = cs.Student.StudentId,
                    Username = cs.Student.Username,
                    FirstName = cs.Student.FirstName,
                    LastName = cs.Student.LastName
                }).ToList();

            var viewModel = new CourseDetailViewModel
            {
                Course = course,
                Students = students
            };

            return View("ViewRoster", viewModel);
        }





    }
}
