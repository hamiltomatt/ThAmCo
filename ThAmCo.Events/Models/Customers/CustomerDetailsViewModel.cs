using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThAmCo.Events.Data;

namespace ThAmCo.Events.Models
{
    public class CustomerDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Last Name")]
        public string Surname { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public List<GuestBooking> Bookings { get; set; }

        public string FullName { get { return string.Format("{0} {1}", FirstName, Surname); } }

        public IEnumerable<GuestEventViewModel> Events { get; set; }
    }
}
