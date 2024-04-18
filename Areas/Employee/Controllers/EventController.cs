using CCD_Attendance.Data;
using CCD_Attendance.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            var events = _dbContext.Events
                .OrderByDescending(e => e.EventDate)
                .ToList();
            return View(events);
        }

        public IActionResult Create()
        {
            return View(new Event());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Events.Add(eventModel);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
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
            if (id != eventModel.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _dbContext.Update(eventModel);
                _dbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(eventModel);
        }

        public IActionResult Delete(int id)
        {
            var eventModel = _dbContext.Events.Find(id);
            if (eventModel == null)
            {
                return NotFound();
            }

            _dbContext.Events.Remove(eventModel);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
