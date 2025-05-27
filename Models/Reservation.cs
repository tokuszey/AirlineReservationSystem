using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OceanicAirlines.Models
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public int UserID { get; set; }
        public int FlightID { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime DepartureDate { get; set; }
        public decimal Price { get; set; }
        public bool IsCanceled { get; set; }
    }
}
