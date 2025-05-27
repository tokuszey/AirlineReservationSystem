using OceanicAirlines.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System;
using System.Web.Mvc;

namespace OceanicAirlines.Controllers
{
    public class FlightsController : Controller
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FlivanDB"].ConnectionString;


    }
}
   


