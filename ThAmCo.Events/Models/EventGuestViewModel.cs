using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class EventGuestViewModel
    {
        public int CustomerId { get; set; }

        public String CustomerName { get; set; }

        public bool Attended { get; set; }
    }
}
