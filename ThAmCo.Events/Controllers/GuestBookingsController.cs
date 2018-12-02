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
    public class GuestBookingsController : Controller
    {
        private readonly EventsDbContext _context;

        public GuestBookingsController(EventsDbContext context)
        {
            _context = context;
        }

        // GET: GuestBookings/Create
        public IActionResult Create([FromQuery] int? customerId)
        {
            if(customerId == null)
            {
                return BadRequest();
            }

            var events = _context.Events.ToList();
            var currentEvents = _context.Guests.Where(gb => gb.CustomerId == customerId).ToList();
            events.RemoveAll(e => currentEvents.Any(gb => gb.EventId == e.Id));

            var gbVm = new GuestBookingViewModel
            {
                CustomerName = _context.Customers.Find(customerId).FullName,
                //CustomerSelectList = new SelectList(_context.Customers, "Id", "FullName", customerId),
                EventSelectList = new SelectList(events, "Id", "Title")
            };
            
            return View(gbVm);
        }

        // POST: GuestBookings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,EventId")] GuestBookingViewModel gbVm)
        {
            if (ModelState.IsValid)
            {
                if (_context.Guests.Any(gb => gb.CustomerId == gbVm.CustomerId && gb.EventId == gbVm.EventId))
                {
                    ModelState.AddModelError(string.Empty, "Customer already booked to this event");
                }
                else
                {
                    var guestBooking = new GuestBooking
                    {
                        CustomerId = gbVm.CustomerId,
                        EventId = gbVm.EventId,
                        Attended = false
                    };
                    _context.Add(guestBooking);
                    await _context.SaveChangesAsync();
                    return base.RedirectToAction("Index", "Customers");
                }                    
            }

            gbVm = new GuestBookingViewModel
            {
                CustomerName = _context.Customers.Find(gbVm.CustomerId).FullName,
                //CustomerSelectList = new SelectList(_context.Customers, "Id", "FullName", gbVm.CustomerId),
                EventSelectList = new SelectList(_context.Events, "Id", "Title")
            };
            return View(gbVm);
        }

        public async Task<IActionResult> MarkAttended(int? id, int? eventId)
        {
            if (id == null || eventId == null)
            {
                return NotFound();
            }

            var booking = await _context.Guests.FindAsync(id, eventId);
            if (booking == null)
            {
                return NotFound();
            }

            booking.Attended = true;
            _context.Update(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Customers", _context.Customers.Find(booking.CustomerId));
        }

        // GET: GuestBookings/Delete/5?eventId=5
        public async Task<IActionResult> Delete(int? id, int? eventId)
        {
            if(id == null || eventId == null)
            {
                return NotFound();
            }

            var booking = await _context.Guests
                .Select(b => new GuestBookingViewModel
                {
                    CustomerId = b.CustomerId,
                    CustomerName = _context.Customers.Where(c => c.Id == id).FirstOrDefault().FullName,
                    EventId = b.EventId,
                    EventName = b.Event.Title,
                }).FirstOrDefaultAsync(c => c.CustomerId == id && c.EventId == eventId);

            if(booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: GuestBookings/Delete/5?eventId=5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int eventId)
        {
            var booking = await _context.Guests.FindAsync(id, eventId);
            if(booking == null)
            {
                return NotFound();
            }

            _context.Guests.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", _context.Events.Find(booking.EventId));
        }

    }
}
