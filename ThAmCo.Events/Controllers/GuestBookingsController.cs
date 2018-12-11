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

        // GET: GuestBookings/Create/5
        /// <summary>
        /// Create a booking for a customer, where it gets all events customer is not registered as a guest on,
        /// populates that into SelectList, and gives to the view model.
        /// </summary>
        /// <param name="customerId">Id of customer</param>
        /// <returns>If success</returns>
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
                EventSelectList = new SelectList(events, "Id", "Title")
            };
            
            return View(gbVm);
        }

        // POST: GuestBookings/Create/5
        /// <summary>
        /// Creates event by binding given values to view model, gives model error if customer is already booked,
        /// projects into new GuestBooking data value if not, and then added to dbcontext and save changes.
        /// If model isn't valid, method will repopulate view model fields as in the "GET" method.
        /// </summary>
        /// <param name="gbVm">View model data is binded to</param>
        /// <returns>If task was successful</returns>
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
                    return RedirectToAction("Index", "Customers");
                }                    
            }

            var events = _context.Events.ToList();
            var currentEvents = _context.Guests.Where(gb => gb.CustomerId == gbVm.CustomerId).ToList();
            events.RemoveAll(e => currentEvents.Any(gb => gb.EventId == e.Id));
            gbVm = new GuestBookingViewModel
            {
                CustomerName = _context.Customers.Find(gbVm.CustomerId).FullName,
                EventSelectList = new SelectList(events, "Id", "Title")
            };
            return View(gbVm);
        }

        //POST: GuestBookings/MarkAttended/5?eventId=5
        /// <summary>
        /// Marks guest booking as being attended by the guest. Takes customer and event ids and finds booking
        /// from them. It then marks it as true, saves changes, and redirects back to event details page they
        /// originated from.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <param name="eventId">Id of event</param>
        /// <returns>If success</returns>
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
            return RedirectToAction("Details", "Customers", _context.Customers.Find(id));
        }

        // GET: GuestBookings/Delete/5?eventId=5
        /// <summary>
        /// Delete booking by using customer and event ids, it finds booking from database and projects it into
        /// view model, which it returns to view.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <param name="eventId">Id of event</param>
        /// <returns>If success</returns>
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
                    CustomerName = b.Customer.FullName,
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
        /// <summary>
        /// Deletes an event by taking customer and event ids, getting booking from db, deleting it, saving
        /// changes, and redirecting to event details.
        /// </summary>
        /// <param name="id">Id of customer</param>
        /// <param name="eventId">Id of event</param>
        /// <returns></returns>
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
            return RedirectToAction("Details", "Events", _context.Customers.Find(eventId));
        }

    }
}
