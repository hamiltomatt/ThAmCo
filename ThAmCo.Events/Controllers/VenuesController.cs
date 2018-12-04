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
    public class VenuesController : Controller
    {

        private readonly EventsDbContext _context;
        private readonly IConfiguration _configuration;

        public VenuesController(EventsDbContext context,
                                IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //GET: Venues/Create/5
        public async Task<IActionResult> Create(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            var availableVenues = new List<VenueGetDto>().AsEnumerable();

            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            string uri = "api/Availability";

            if (@event != null)
            {
                uri = uri + "?eventType=" + @event.TypeId + "&beginDate=" + @event.Date.ToString("d-MMM-yyyy")
                            + "&endDate=" + @event.Date.AddDays(7).ToString("d-MMM-yyyy");
            }

            HttpResponseMessage response = await client.GetAsync(uri);
            if (response.IsSuccessStatusCode)
            {
                availableVenues = await response.Content.ReadAsAsync<IEnumerable<VenueGetDto>>();
            }
            else
            {
                return NotFound();
            }

            var venues = availableVenues.Select(v => new VenueEventViewModel
            {
                EventId = id,
                VenueCode = v.Code,
                VenueName = v.Name,
                Description = v.Description,
                Capacity = v.Capacity,
                Date = v.Date,
                CostPerHour = v.CostPerHour
            }).ToList();

            var eventVenues = new EventVenueViewModel
            {
                EventId = id,
                EventName = @event.Title,
                EventDate = @event.Date,
                Venues = venues
            };

            return View(eventVenues);
        }

        //POST: Venues/Create/5?venueCode=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("eventId, venueCode")] VenueEventViewModel vEvm)
        {
            if(ModelState.IsValid)
            {

                var @event = await _context.Events.FindAsync(vEvm.EventId);

                var dto = new ReservationPostDto
                {
                    EventDate = @event.Date,
                    VenueCode = vEvm.VenueCode,
                    StaffId = "aa"
                };

                HttpClient client = new HttpClient();
                client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
                client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

                HttpResponseMessage postResponse = await client.PostAsJsonAsync("api/Reservations", dto);

                postResponse.EnsureSuccessStatusCode();
            }

            return RedirectToAction("Details", "Events", vEvm.EventId);
 
        }
    }
}