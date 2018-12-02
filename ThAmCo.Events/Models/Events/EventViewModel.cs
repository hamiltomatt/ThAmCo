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

        [Required(ErrorMessage = "Date is required")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:g}")]
        public TimeSpan? Duration { get; set; }

        [Required(ErrorMessage = "Event type is required")]
        [Display(Name = "Type")]
        public string TypeId { get; set; }

        public SelectList TypeSelectList { get; set; }
    }
}
