using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CCD_Attendance.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class AttendanceController : Controller
    {
        private readonly ccdDBContext _dbContext;

        public AttendanceController(ccdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Upload(int eventId)
        {
            var model = new UploadAttendanceViewModel
            {
                EventId = eventId
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadAttendanceViewModel model)
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

            var students = new List<Student>();
            var attendances = new List<Attendance>();

            using (var stream = new StreamReader(model.File.OpenReadStream()))
            {
                string headerLine = await stream.ReadLineAsync();
                var headers = headerLine.Split(',');
                if (headers.Length != 7 || headers[2].Trim() != "Email Address" || headers[3].Trim() != "Username")
                {
                    ModelState.AddModelError("File", "Incorrect CSV format.");
                    return View(model);
                }

                while (!stream.EndOfStream)
                {
                    var line = await stream.ReadLineAsync();
                    var data = line.Split(',');

                    var username = data[3].Trim();
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

                    attendances.Add(new Attendance { StudentId = student.StudentId, EventId = model.EventId });
                }
            }

            var existingRecords = _dbContext.Attendances.Where(a => a.EventId == model.EventId);
            _dbContext.Attendances.RemoveRange(existingRecords);
            await _dbContext.SaveChangesAsync();

            _dbContext.Attendances.AddRange(attendances);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("DetailViewAttendance", "Event", new { eventId = model.EventId });
        }





        [HttpGet]
        public IActionResult UpdateAttendance(int eventId)
        {
            var model = new UploadAttendanceViewModel
            {
                EventId = eventId
            };
            return View(model);  // Now pointing to the new view
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAttendance(UploadAttendanceViewModel model)
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

            var validationErrors = false;

            using (var stream = new StreamReader(model.File.OpenReadStream()))
            {
                string headerLine = await stream.ReadLineAsync();
                var headers = headerLine.Split(',');
                if (headers.Length != 7 || headers[2].Trim() != "Email Address" || headers[3].Trim() != "Username")
                {
                    ModelState.AddModelError("File", "Incorrect CSV format.");
                    validationErrors = true;
                }

                if (!validationErrors)
                {
                    var students = new List<Student>();
                    var attendances = new List<Attendance>();

                    while (!stream.EndOfStream)
                    {
                        var line = await stream.ReadLineAsync();
                        var data = line.Split(',');

                        var username = data[3].Trim();
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
                            // Do not save changes yet
                        }

                        attendances.Add(new Attendance { StudentId = student.StudentId, EventId = model.EventId });
                    }

                    var existingRecords = _dbContext.Attendances.Where(a => a.EventId == model.EventId);
                    _dbContext.Attendances.RemoveRange(existingRecords);
                    await _dbContext.SaveChangesAsync();

                    _dbContext.Attendances.AddRange(attendances);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (validationErrors)
            {
                return View(model);
            }

            return RedirectToAction("DetailViewAttendance", "Event", new { eventId = model.EventId });
        }




    }
}
