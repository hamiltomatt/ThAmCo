using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [Display(Name = "Venue")]
        public string VenueName { get; set; }

        [Display(Name = "Guests")]
        public int NoOfGuests { get; set; }

        [DataType(DataType.Duration), DisplayFormat(DataFormatString = @"{0:hh\:mm}")]
        public TimeSpan? Duration { get; set; }

        [Required(ErrorMessage = "Event type is required")]
        public string Type { get; set; }

        public SelectList TypeSelectList { get; set; }
    }
}
