using OceanicAirlines.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace OceanicAirlines.Controllers
{
    public class AccountController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FlivanDB"].ConnectionString;

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Users (UserName, Email, PasswordHash) VALUES (@UserName, @Email, @PasswordHash)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage ="Please fill in all fields.";
                return View();
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT UserID, UserName, PasswordHash FROM Users WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string storedPassword = reader["PasswordHash"].ToString();
                    if (storedPassword == password)
                    {
                        Session["UserID"] = reader["UserID"];
                        Session["UserName"] = reader["UserName"];
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "The password is incorrect.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found. Please register.";
                    return View();
                }
            }
        }

        // Çıkış yapma
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult Purchase(int flightId)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Flight flight = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Flights WHERE FlightID = @FlightID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FlightID", flightId);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    flight = new Flight
                    {
                        FlightID = flightId,
                        FromLocation = reader["FromLocation"].ToString(),
                        ToLocation = reader["ToLocation"].ToString(),
                        DepartureDate = Convert.ToDateTime(reader["DepartureDate"]),
                        ArrivalDate = Convert.ToDateTime(reader["ArrivalDate"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    };
                }
            }

            if (flight == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(flight);
        }
        [HttpPost]
        public ActionResult Purchase(int flightId, string cardNumber, string cardHolderName, string expiryDate, string cvv)
        {
            
            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cardHolderName) || string.IsNullOrEmpty(expiryDate) || string.IsNullOrEmpty(cvv))
            {
                ViewBag.ErrorMessage = "Please fill out all card information.";
                return View();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Reservations (UserID, FlightID, ReservationDate) VALUES (@UserID, @FlightID, @ReservationDate)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@UserID", (int)Session["UserID"]); 
                cmd.Parameters.AddWithValue("@FlightID", flightId); 
                cmd.Parameters.AddWithValue("@ReservationDate", DateTime.Now); 

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            ViewBag.SuccessMessage = "Your payment has been completed successfully and your reservation has been created!";
            return RedirectToAction("Success"); 
        }



        [HttpGet]
        public ActionResult MyReservations()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Reservation> reservations = new List<Reservation>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT r.ReservationID, f.FromLocation, f.ToLocation, f.DepartureDate, f.Price 
            FROM Reservations r
            INNER JOIN Flights f ON r.FlightID = f.FlightID
            WHERE r.UserID = @UserID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", Session["UserID"]);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    reservations.Add(new Reservation
                    {
                        ReservationID = Convert.ToInt32(reader["ReservationID"]),
                        FromLocation = reader["FromLocation"].ToString(),
                        ToLocation = reader["ToLocation"].ToString(),
                        DepartureDate = Convert.ToDateTime(reader["DepartureDate"]),
                        Price = Convert.ToDecimal(reader["Price"])
                    });
                }
            }

            return View(reservations);
        }



        public ActionResult Success()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CancelReservation(int reservationId)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Reservations WHERE ReservationID = @ReservationID AND UserID = @UserID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReservationID", reservationId);
                cmd.Parameters.AddWithValue("@UserID", (int)Session["UserID"]);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("MyReservations");
        }

    }
}
