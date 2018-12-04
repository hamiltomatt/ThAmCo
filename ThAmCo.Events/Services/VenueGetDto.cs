using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class VenueGetDto
    {
        public string VenueCode { get; set; }

        public String VenueName { get; set; }

        public String Description { get; set; }

        public int Capacity { get; set; }

        public DateTime Date { get; set; }

        public double CostPerHour { get; set; }
    }
}
