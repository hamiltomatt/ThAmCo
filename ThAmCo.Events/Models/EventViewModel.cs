using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Events.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string TypeId { get; set; }

        public SelectList TypeSelectList { get; set; }
    }
}
