using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class GuestBookingViewModel
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }

        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        public SelectList CustomerSelectList { get; set; }

        public SelectList EventSelectList { get; set; }
    }
}
