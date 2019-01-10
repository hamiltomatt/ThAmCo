using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class StaffEventViewModel
    {
        public int StaffId { get; set; }

        public int EventId { get; set; }

        [Display(Name = "Name")]
        public string EventName { get; set; }

        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EventDate { get; set; }

    }
}
