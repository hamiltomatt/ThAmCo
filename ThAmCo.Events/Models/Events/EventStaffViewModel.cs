using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class EventStaffViewModel
    {
        public int StaffId { get; set; }

        [Display(Name = "Name")]
        public string StaffName { get; set; }

        public int EventId { get; set; }

    }
}
