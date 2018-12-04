using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class VenueEventViewModel
    {
        public int EventId { get; set; }

        public string VenueCode { get; set; }

        [Display(Name = "Venue")]
        public String VenueName { get; set; }

        public String Description { get; set; }

        public int Capacity { get; set; }

        public DateTime Date { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "0:C")]
        [Display(Name = "Cost per Hour")]
        public double CostPerHour { get; set; }
    }
}
