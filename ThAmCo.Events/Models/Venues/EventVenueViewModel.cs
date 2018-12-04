using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class EventVenueViewModel
    {
        public int EventId { get; set; }

        public string EventName { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EventDate { get; set; }

        public IEnumerable<Models.VenueEventViewModel> Venues { get; set; }
    }
}
