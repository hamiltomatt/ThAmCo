using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThAmCo.Catering.Data
{
    public class CateringDbContext
    {
        public DbSet<FoodBooking> Bookings { get; set; }

        public DbSet<Menu> Menus { get; set; }

        public CateringDbContext()
        {

        }
    }
}
