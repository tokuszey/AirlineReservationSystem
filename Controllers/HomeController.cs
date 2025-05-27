using OceanicAirlines.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Web.Mvc;

namespace OceanicAirlines.Controllers
{
    public class HomeController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FlivanDB"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            List<string> cities = new List<string>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT CityName FROM Cities";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cities.Add(reader["CityName"].ToString());
                }
            }
            ViewBag.Cities = cities; 
            return View();
        }

        [HttpPost]
        public ActionResult Index(string fromLocation, string toLocation, DateTime? departureDate)
        {
            List<Flight> flights = new List<Flight>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Flights WHERE FromLocation = @FromLocation AND ToLocation = @ToLocation AND DepartureDate >= @DepartureDate";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FromLocation", fromLocation);
                cmd.Parameters.AddWithValue("@ToLocation", toLocation);
                cmd.Parameters.AddWithValue("@DepartureDate", departureDate ?? DateTime.Now); 

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    flights.Add(new Flight
                    {
                        FlightID = Convert.ToInt32(reader["FlightID"]),
                        FromLocation = reader["FromLocation"].ToString(),
                        ToLocation = reader["ToLocation"].ToString(),
                        DepartureDate = Convert.ToDateTime(reader["DepartureDate"]),
                        ArrivalDate = Convert.ToDateTime(reader["ArrivalDate"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    });
                }
            }

            if (flights.Count == 0)
            {
                ViewBag.Message = "Flight not found.";
            }

            return View("SearchResults", flights); 
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

    }
}

