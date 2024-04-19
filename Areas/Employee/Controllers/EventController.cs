using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;


namespace CCD_Attendance.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Employee")]
    public class EventController : Controller
    {
        private readonly ccdDBContext _dbContext;

        public EventController(ccdDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            //dashboard view
            return View();
        }

        public IActionResult MyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myEvents = _dbContext.Events
                .Where(e => e.UserId == userId && e.ApprovalStatus)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            return View(myEvents);
        }

        public IActionResult AllEvents()
        {
            var eventRequests = _dbContext.Events
                .Where(e => !e.ApprovalStatus)
                .OrderByDescending(e => e.EventDate)
                .ToList();
            var approvedEvents = _dbContext.Events
                .Where(e => e.ApprovalStatus)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            ViewBag.ApprovedEvents = approvedEvents; // Use ViewBag to pass approved events to the view
            return View(eventRequests);
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
            eventModel.ApprovalStatus = true; // Automatically set to true for employees

            // Validate existing event names on the same date
            var exists = _dbContext.Events.Any(e => e.EventName == eventModel.EventName && e.EventDate == eventModel.EventDate);
            if (exists)
            {
                ModelState.AddModelError("EventName", "An event with this name and date already exists.");
                return View(eventModel); // Return here to show the error in the view
            }


            _dbContext.Events.Add(eventModel);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(MyEvents));
        }

        public IActionResult Edit(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event eventModel)
        {
            var existingEvent = _dbContext.Events.Find(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.EventName = eventModel.EventName;
            existingEvent.EventDate = eventModel.EventDate;
            existingEvent.EventDetails = eventModel.EventDetails;
            existingEvent.EventNotesCCD = eventModel.EventNotesCCD; // Allow editing of CCD notes
            existingEvent.ApprovalStatus = true;

            _dbContext.Update(existingEvent);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(AllEvents));
        }


        //Edit for the MyEvents page
        public IActionResult EditMyEvents(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel == null)
            {
                return NotFound();
            }
            return View(eventModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMyEvents(int id, Event eventModel)
        {
            var existingEvent = _dbContext.Events.Find(id);
            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.EventName = eventModel.EventName;
            existingEvent.EventDate = eventModel.EventDate;
            existingEvent.EventDetails = eventModel.EventDetails;
            existingEvent.EventNotesCCD = eventModel.EventNotesCCD; // Allow editing of CCD notes
            existingEvent.ApprovalStatus = true;

            _dbContext.Update(existingEvent);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(MyEvents));
        }

        public IActionResult Approve(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel != null)
            {
                eventModel.ApprovalStatus = true;
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(AllEvents));
        }

        public IActionResult Deny(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel != null)
            {
                _dbContext.Events.Remove(eventModel);
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(AllEvents));
        }

        public IActionResult Delete(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel != null)
            {
                _dbContext.Events.Remove(eventModel);
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(AllEvents));
        }

        public IActionResult DeleteMyEvents(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel != null)
            {
                _dbContext.Events.Remove(eventModel);
                _dbContext.SaveChanges();
            }
            return RedirectToAction(nameof(MyEvents));
        }
    }
}
