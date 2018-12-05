using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Controllers
{
    public class StaffingsController : Controller
    {
        private readonly EventsDbContext _context;

        /// <summary>
        /// Constructor, supplies staff with database context
        /// </summary>
        /// <param name="context">EF database context</param>
        public StaffingsController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: Staffings/Create/5
        /// <summary>
        /// Creates staffing of event, by providing event id. It will remove staff currently assigned to event.
        /// It will then project the list into SelectList in view model, and sends to view.
        /// </summary>
        /// <param name="eventId">Id of event</param>
        /// <returns>If success</returns>
        public IActionResult Create([FromQuery] int? eventId)
        {
            if(eventId == null)
            {
                return BadRequest();
            }

            //Removes staff members that are already assigned to given event
            var staff = _context.Staff.ToList();
            var currentStaff = _context.Staffings.Where(s => s.EventId == eventId).ToList();
            staff.RemoveAll(s => currentStaff.Any(sf => sf.EventId == eventId));

            ////Removes staff that have bookings on the same day as given event
            //var date = _context.Events.Find(eventId).Date;
            //var unavailableStaff = _context.Staffings.Where(s => s.Event.Date == date).ToList();
            //staff.RemoveAll(s => unavailableStaff.Any(sf => sf.Event.Date == date));

            var sVm = new StaffingViewModel
            {
                EventName = _context.Events.Find(eventId).Title,
                StaffSelectList = new SelectList(staff, "Id", "FullName")
            };

            return View(sVm);
        }

        // POST: Staffings/Create/5
        /// <summary>
        /// Gets data from view and binds it to view model, creates new staffing object, and posts to db.
        /// Else, it repopulates the fields for re-entry.
        /// </summary>
        /// <param name="sVm">Model where binded values will land</param>
        /// <returns>If success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StaffId,EventId")] StaffingViewModel sVm)
        {
            if (ModelState.IsValid)
            {
                if(_context.Staffings.Any(s => s.EventId == sVm.EventId && s.StaffId == sVm.StaffId))
                {
                    ModelState.AddModelError(string.Empty, "Staff is already booked on this event");
                }
                else
                {
                    var staffing = new Staffing
                    {
                        StaffId = sVm.StaffId,
                        EventId = sVm.EventId
                    };

                    _context.Add(staffing);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", "Events", _context.Events.Find(sVm.EventId));
                }
            }

            //Removes staff members that are already assigned to given event
            var staff = _context.Staff.ToList();
            var currentStaff = _context.Staffings.Where(s => s.EventId == sVm.EventId).ToList();
            staff.RemoveAll(s => currentStaff.Any(sf => sf.EventId == sVm.EventId));
            ////Removes staff that have bookings on the same day as given event
            //var date = _context.Events.Find(sVm.EventId).Date;
            //var unavailableStaff = _context.Staffings.Where(s => s.Event.Date == date).ToList();
            //staff.RemoveAll(s => unavailableStaff.Any(sf => sf.Event.Date == date));
            sVm = new StaffingViewModel
            {
                EventName = _context.Events.Find(sVm.EventId).Title,
                StaffSelectList = new SelectList(staff, "Id", "FullName")
            };

            return View(sVm);
        }

        // GET: Staffings/Delete/5?eventId=5
        /// <summary>
        /// Delete a staffing, by using the staffid and eventid to get the staffing to be deleted, and 
        /// returns to view.
        /// </summary>
        /// <param name="id">Id for staff</param>
        /// <param name="eventId">Id for event</param>
        /// <returns>If operation is a success</returns>
        public async Task<IActionResult> Delete(int? id, int? eventId)
        {
            if (id == null || eventId == null)
            {
                return NotFound();
            }

            var staffing = await _context.Staffings
                .Select(s => new StaffingViewModel
                {
                    StaffId = s.StaffId,
                    StaffName = s.Staff.FullName,
                    EventId = s.EventId,
                    EventName = s.Event.Title
                })
                .FirstOrDefaultAsync(m => m.StaffId == id && m.EventId == eventId);

            if (staffing == null)
            {
                return NotFound();
            }

            return View(staffing);
        }

        // POST: Staffings/Delete/5?eventId=5
        /// <summary>
        /// Gets ids from delete view, and gets specified object from db. It then removes and saves changes.
        /// </summary>
        /// <param name="id">Staff id</param>
        /// <param name="eventId">Event id</param>
        /// <returns>If success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int eventId)
        {
            var staffing = await _context.Staffings.FindAsync(id, eventId);
            if(staffing == null)
            {
                return NotFound();
            }

            _context.Staffings.Remove(staffing);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", _context.Events.Find(eventId));
        }

    }
}
