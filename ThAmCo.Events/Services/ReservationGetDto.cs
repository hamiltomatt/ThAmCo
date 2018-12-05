using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class ReservationGetDto
    {
        public string Reference { get; set; }

        public DateTime EventDate { get; set; }

        public string VenueCode { get; set; }

        public string VenueName { get; set; }

        public int VenueCapacity { get; set; }

        public double VenueCostPerHour { get; set; }

        public DateTime WhenMade { get; set; }

        public string StaffId { get; set; }
    }
}
