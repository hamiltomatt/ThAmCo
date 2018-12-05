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

        public HttpClient getVenuesClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        //GET: Venues/Book/5
        public async Task<IActionResult> Book(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            var availableVenues = new List<VenueGetDto>().AsEnumerable();

            HttpClient client = getVenuesClient();

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

        //POST: Venues/BookConfirmed/5?venueCode=X&date=X
        public async Task<IActionResult> BookConfirmed(int? id, string venueCode, DateTime date)
        {
            if(id == null || venueCode == null || date == null)
            {
                return NotFound();
            }
            var @event = await _context.Events.FindAsync(id);

            HttpClient client = getVenuesClient();

            if(@event.Reservation != null)
            {
                try
                {
                    HttpResponseMessage deleteResponse = await client.DeleteAsync("api/Reservations/" + @event.Reservation);
                    deleteResponse.EnsureSuccessStatusCode();
                }
                catch(Exception)
                {
                    return NotFound();
                }
            }


            var dto = new ReservationPostDto
            {
                EventDate = date,
                VenueCode = venueCode,
                StaffId = "1"
            };

            HttpResponseMessage postResponse = await client.PostAsJsonAsync("api/Reservations", dto);
            if(postResponse.IsSuccessStatusCode)
            {
                @event.Date = date;
                @event.Reservation = $"{venueCode}{date:yyyyMMdd}";
            }
            else
            {
                return NotFound();
            }

            _context.Events.Update(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", @event);
        }

    }
}