using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventsDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor which takes the context for the entity framework model, and the configuration file
        /// which specifies the location of the Venues API.
        /// </summary>
        /// <param name="context">Database EF context</param>
        /// <param name="configuration">Local appsettings file</param>
        public EventsController(EventsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets list of all event types from web service, by specifying its URI and using GetAsync,
        /// which will bring down all types into a list, which is captured by an IEnumerable of the
        /// eventTypeGetDto data transfer object.
        /// </summary>
        /// <returns>A list of EventTypeGetDto objects</returns>
        public async Task<IEnumerable<EventTypeGetDto>> getEventTypes()
        {
            var eventTypes = new List<EventTypeGetDto>().AsEnumerable();

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            HttpResponseMessage response = await client.GetAsync("api/EventTypes");
            if (response.IsSuccessStatusCode)
            {
                eventTypes = await response.Content.ReadAsAsync<IEnumerable<EventTypeGetDto>>();
                return eventTypes;
            }
            else
            {
                throw new Exception();
            }
        }

        /// <summary>
        /// Gets venue name of a specific reservation by giving it a reference string, which is used to query
        /// the web service, where the data read in will be read out into the ReservationGetDto type object.
        /// </summary>
        /// <param name="reference">The reference of a reservation</param>
        /// <returns>String of venue name</returns>
        public async Task<string> getVenueName(string reference)
        {
            var reservation = new ReservationGetDto();
            if (reference != null)
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                HttpResponseMessage response = await client.GetAsync("api/Reservations/" + reference);
                if (response.IsSuccessStatusCode)
                {
                    reservation = await response.Content.ReadAsAsync<ReservationGetDto>();
                    return reservation.VenueName;
                }
                else
                {
                    throw new Exception();
                }
            }
            return null;
        }

        // GET: Events
        /// <summary>
        /// Gets index of events by projecting all events from EF context into event view model, and 
        /// gives it to view. Also uses eventTypes to get the venue title of event
        /// </summary>
        /// <returns>If was successful</returns>
        public async Task<IActionResult> Index()
        {
            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var events = await _context.Events.Where(e => e.IsActive).Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                VenueName = getVenueName(e.Reservation).Result ?? "No Venue",
                NoOfGuests = e.Bookings.Count,
                Duration = e.Duration,
                Type = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title
            }).ToListAsync();

            return View(events);
        }

        // GET: Events/Details/5
        /// <summary>
        /// Gets detail of event by providing with event id, then projects database information into
        /// event details view model, which will modify the result for the view. It will also populate
        /// the Guests list (event's guests) and its Staff list (staff working event), and then send to view
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var @event = await _context.Events.Where(e => e.IsActive).Select(e => new EventDetailsViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                VenueName = getVenueName(e.Reservation).Result ?? "No Venue",
                NoOfGuests = e.Bookings.Count,
                Duration = e.Duration,
                Type = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title,
                Guests = e.Bookings.Select(b => new EventGuestViewModel
                {
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer.FullName,
                    EventId = b.EventId,
                    Attended = b.Attended
                }),
                Staff = e.Staffings.Select(s => new EventStaffViewModel
                {
                    StaffId = s.StaffId,
                    StaffName = s.Staff.FullName,
                    EventId = s.EventId
                })
            }).FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.Staff.Any())
            {
                if (!@event.Staff.Any(s => _context.Staff.Find(s.StaffId).IsFirstAider == true))
                {
                    ModelState.AddModelError("noFirstAider", "There is no first-aider assigned");
                }

                var firstAiderCount = (@event.Staff.Where(s => _context.Staff.Find(s.StaffId).IsFirstAider)).Count();
                if ((firstAiderCount / @event.Staff.Count()) < 0.1)
                {
                    ModelState.AddModelError("moreFirstAiders", "At least 1 first-aider needs to be assigned" +
                        " per 10 guests");
                }
            }

            return View(@event);
        }

        // GET: Events/Create
        /// <summary>
        /// Creates empty event form with SelectList populated with EventTypes, and sent to view
        /// </summary>
        /// <returns>If success</returns>
        public async Task<IActionResult> Create()
        {
            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var typeSelectList = new SelectList(eventTypes, "Id", "Title");

            return View(new EventViewModel() { TypeSelectList = typeSelectList } );
        }

        // POST: Events/Create
        /// <summary>
        /// Binds data to view model, projects into event, adds event to context and saves changes to db
        /// </summary>
        /// <param name="eventVm">View model recieved from create event view</param>
        /// <returns>If success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Duration,Type")] EventViewModel eventVm)
        {
            if (ModelState.IsValid)
            {
                var @event = new Event()
                {
                    IsActive = true,
                    Title = eventVm.Title,
                    Date = eventVm.Date,
                    Duration = eventVm.Duration,
                    TypeId = eventVm.Type
                };

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            eventVm.TypeSelectList = new SelectList(eventTypes, "Id", "Title");

            return View(eventVm);
        }

        // GET: Events/Edit/5
        /// <summary>
        /// Edits an event by getting event from dbcontext, and projects into new event edit viewmodel for view
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null || !@event.IsActive)
            {
                return NotFound();
            }

            var eventVm = new EventEditViewModel()
            {
                Id = @event.Id,
                Title = @event.Title,
                Duration = @event.Duration
            };

            return View(eventVm);
        }

        // POST: Events/Edit/5
        /// <summary>
        /// Edits event by getting database object and updating just the title and duration, and saving changes.
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <param name="eventVm">Object with new values</param>
        /// <returns>If success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Duration")] EventEditViewModel eventVm)
        {
            if (id != eventVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var @event = await _context.Events.FindAsync(id);
                    @event.IsActive = true;
                    @event.Title = eventVm.Title;
                    @event.Duration = eventVm.Duration;

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(eventVm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(eventVm);
        }

        // GET: Events/Delete/5
        /// <summary>
        /// Delete an event, by projecting data into model and pushing to view.
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If success</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var @event = await _context.Events.Where(e => e.IsActive).Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                VenueName = getVenueName(e.Reservation).Result ?? "No Venue",
                NoOfGuests = e.Bookings.Count,
                Duration = e.Duration,
                Type = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title
            }).FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        /// <summary>
        /// Deletes an event, by getting event from dbcontext, removing and saving changes to db.
        /// Method also calls reservation web service to clear any venues it may have booked for the future.
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            if (@event.Reservation != null)
            {
                try
                {
                    HttpResponseMessage deleteResponse = await client.DeleteAsync("api/Reservations/"
                                                            + @event.Reservation);
                    deleteResponse.EnsureSuccessStatusCode();
                }
                catch(Exception)
                {
                    return NotFound();
                }
            }

            //_context.Events.Remove(@event);
            @event.IsActive = false;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checking if an event for an id exists in the current database context
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If event does exist</returns>
        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
