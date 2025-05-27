using System;

namespace OceanicAirlines.Models
{
    public class Flight
    {
        public int FlightID { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public decimal Price { get; set; }
    }
}
