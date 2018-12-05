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

        /// <summary>
        /// Constructor which takes the context for the entity framework model, and the configuration file
        /// which specifies the location of our WebAPI services.
        /// </summary>
        /// <param name="context">Database EF context</param>
        /// <param name="configuration">Local appsettings file</param>
        public VenuesController(EventsDbContext context,
                                IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Gets client which can connect to venues service, and returns it
        /// </summary>
        /// <returns>HttpClient object</returns>
        public HttpClient getVenuesClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new System.Uri(_configuration["VenuesBaseURI"]);
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        //GET: Venues/Book/5
        /// <summary>
        /// Book a venue for an eventid, where it gets the event from db, specifies client and uri and
        /// queries to get available venues in a week's range of the specified event date. It then projects the
        /// venue data into VenueEventViewModel as a list, and the event data into EventVenueViewModel, storing
        /// the VEVMs inside of its view model. The EventVenue model is then sent to the View, where a partial view
        /// communicates with the venue list.
        /// </summary>
        /// <param name="id">Id of event</param>
        /// <returns>If success</returns>
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
        /// <summary>
        /// When a booking of a specific venue is confirmed, the event id, the venue code and the date are all
        /// returned from the view, where it attempts to find the event from the database. If it finds a reservation
        /// already exists, it will call the delete reservation method in the venues API, which will remove the
        /// current booking. A new ReservationPostDto is then created, which if posted, will trigger an update of
        /// the event's date and reservation to reflect the new booking.
        /// </summary>
        /// <param name="id">Event id</param>
        /// <param name="venueCode">Code for venue</param>
        /// <param name="date">Date of event</param>
        /// <returns>If success</returns>
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