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
    public class EventController : Controller
    {
        private readonly ccdDBContext _dbContext;

        public EventController(ccdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // This view now acts as a dashboard to navigate to other views
            return View();
        }

        public IActionResult MyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = _dbContext.Events
                .Where(e => e.UserId == userId && e.ApprovalStatus)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(events);
        }

        public IActionResult AllEvents()
        {
            var events = _dbContext.Events
                .Where(e => e.ApprovalStatus)
                .Include(e => e.ApplicationUser)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(events);
        }

        public IActionResult MyRequests()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var events = _dbContext.Events
                .Where(e => e.UserId == userId && !e.ApprovalStatus)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(events); // Reuse the MyEvents view for this purpose
        }

        public IActionResult Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Event eventModel = new Event();
            eventModel.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);


            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event eventModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            eventModel.UserId = userId;
            eventModel.ApplicationUser = _dbContext.ApplicationUsers.Find(userId);

            // Validate existing event names on the same date
            var exists = _dbContext.Events.Any(e => e.EventName == eventModel.EventName && e.EventDate == eventModel.EventDate);
            if (exists)
            {
                ModelState.AddModelError("EventName", "An event with this name and date already exists.");
                return View(eventModel); // Return here to show the error in the view
            }

            _dbContext.Events.Add(eventModel);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(MyRequests));
        }

       

        public IActionResult Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventModel = _dbContext.Events.FirstOrDefault(e => e.EventId == id && e.UserId == userId);
            if (eventModel == null)
            {
                return NotFound();
            }

            _dbContext.Events.Remove(eventModel);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(MyRequests));
        }
    }
}
