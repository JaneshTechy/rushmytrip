using FlightServices.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TravelWeb.Helper;
using TravelWeb.Models;

namespace TravelWeb.Controllers
{
   
    public class Acknowledge : Controller
    {
        [Route("acknowledge")]
        public IActionResult Index(string @ref, string searchId)
        {
            string BookingId = "";
            //HttpContext.Session.SetObjectAsJson("IsValid", "FALSE");
            //string jsonData =AplicationHelper.Get("AcknowledgeData", @ref);
            //AcknowledgeData ackdata = JsonConvert.DeserializeObject<AcknowledgeData>(jsonData);

            AcknowledgeData ackdata = AplicationHelper.Get<AcknowledgeData>(searchId, "AcknowledgeData");

            //AcknowledgeData ackdata = HttpContext.Session.GetObjectFromJson<AcknowledgeData>("AcknowledgeData");
            if (ackdata == null)
            {
                //return Redirect("/");
            }

            ViewBag.Reference = ackdata.Reference;
            ViewBag.TravelDate = ackdata.TravelDate;
            ViewBag.PassengerName = ackdata.PassengerName;
            ViewBag.PaidTo = ackdata.PaidTo;
            ViewBag.PaymentMethod = ackdata.PaymentMethod;
            ViewBag.Subject = ackdata.Subject;
            ViewBag.AmountPaid = ackdata.AmountPaid;
            HttpContext.Session.Clear();

            return View();
        }
        
    }
}
