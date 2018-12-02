using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class GuestEventViewModel
    {
        public int EventId { get; set; }

        [Display(Name = "Name")]
        public string EventName { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EventDate { get; set; }

        [Display(Name = "Attended?")]
        public bool Attended { get; set; }
    }
}
