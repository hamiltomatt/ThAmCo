using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventsDbContext _context;

        public EventsController(EventsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventTypeGetDto>> getEventTypes()
        {
            var eventTypes = new List<EventTypeGetDto>().AsEnumerable();

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri("http://localhost:23652");
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

        // GET: Events
        public async Task<IActionResult> Index()
        {
            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var events = await _context.Events.Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                Duration = e.Duration,
                TypeId = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title
            }).ToListAsync();

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var @event = await _context.Events.Select(e => new EventDetailsViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                Duration = e.Duration,
                TypeId = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title,
                Bookings = e.Bookings.Select(b => new EventGuestViewModel
                {
                    CustomerId = b.CustomerId,
                    CustomerName = b.Customer.FullName,
                    Attended = b.Attended
                })
            }).FirstOrDefaultAsync(e => e.Id == id);


            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var typeSelectList = new SelectList(eventTypes, "Id", "Title");

            return View(new EventViewModel() { TypeSelectList = typeSelectList } );
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Date,Duration,TypeId")] EventViewModel eventVm)
        {
            if (ModelState.IsValid)
            {
                var @event = new Event()
                {
                    Title = eventVm.Title,
                    Date = eventVm.Date,
                    Duration = eventVm.Duration,
                    TypeId = eventVm.TypeId
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
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
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            IEnumerable<EventTypeGetDto> eventTypes = await getEventTypes();

            var @event = await _context.Events.Select(e => new EventViewModel
            {
                Id = e.Id,
                Title = e.Title,
                Date = e.Date,
                Duration = e.Duration,
                TypeId = eventTypes.Where(t => t.Id == e.TypeId).FirstOrDefault().Title
            }).FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
