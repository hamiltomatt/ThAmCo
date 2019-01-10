using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Data
{
    public class Event
    {
        public int Id { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }

        [Required, MaxLength(3), MinLength(3)]
        public string TypeId { get; set; }

        public string Reservation { get; set; }

        public List<GuestBooking> Bookings { get; set; }

        public List<Staffing> Staffings { get; set; }
    }
}
