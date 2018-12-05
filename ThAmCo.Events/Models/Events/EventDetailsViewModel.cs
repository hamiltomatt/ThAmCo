using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class EventDetailsViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [Display(Name = "Venue")]
        public string VenueName { get; set; }

        [Display(Name = "Guests")]
        public int NoOfGuests { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? Duration { get; set; }

        [Required, MaxLength(3), MinLength(3)]
        public string Type { get; set; }

        public IEnumerable<EventGuestViewModel> Guests { get; set; }

        public IEnumerable<EventStaffViewModel> Staff { get; set; }
    }
}
