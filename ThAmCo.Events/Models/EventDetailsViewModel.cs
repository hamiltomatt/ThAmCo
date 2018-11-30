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

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }

        [Display(Name = "Type")]
        [Required, MaxLength(3), MinLength(3)]
        public string TypeId { get; set; }

        public IEnumerable<EventGuestViewModel> Bookings { get; set; }
    }
}
