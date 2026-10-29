using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelWeb.Models
{
    //Class for cardvalidaton
    public class cardValidation
    {
        public int bin { get; set; }
        public string bank { get; set; }
        public string card { get; set; }
        public string type { get; set; }
        public string level { get; set; }
        public string country { get; set; }
        public string countrycode { get; set; }
        public string website { get; set; }
        public string phone { get; set; }
        public string valid { get; set; }
        public string error { get; set; }
        public string message { get; set; }
    }
}
