using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Events.Models
{
    public class StaffingViewModel
    {
        [Display(Name = "Staff")]
        public int StaffId { get; set; }

        [Display(Name = "Staff Name")]
        public string StaffName { get; set; }

        [Display(Name = "Event")]
        public int EventId { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }

        public SelectList StaffSelectList { get; set; }
    }
}
