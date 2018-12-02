using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ThAmCo.Venues.Models
{
    public class ReservationGetDto
    {
        public string Reference { get; set; }

        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        public string VenueCode { get; set; }

        public string VenueName { get; set; }

        public int VenueCapacity { get; set; }

        public double VenueCostPerHour { get; set; }

        public DateTime WhenMade { get; set; }

        public string StaffId { get; set; }

        public static ReservationGetDto FromModel(Data.Reservation reservation)
        {
            return new ReservationGetDto
            {
                Reference = reservation.Reference,
                EventDate = reservation.EventDate,
                VenueCode = reservation.VenueCode,
                VenueName = reservation.Availability.Venue.Name,
                VenueCapacity = reservation.Availability.Venue.Capacity,
                VenueCostPerHour = reservation.Availability.CostPerHour,
                WhenMade = reservation.WhenMade,
                StaffId = reservation.StaffId
            };
        }
    }
}