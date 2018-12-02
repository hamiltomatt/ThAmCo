using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class EventGuestViewModel
    {
        public int CustomerId { get; set; }

        [Display(Name="Name")]
        public String CustomerName { get; set; }

        public int EventId { get; set; }

        [Display(Name = "Attended?")]
        public bool Attended { get; set; }
    }
}
