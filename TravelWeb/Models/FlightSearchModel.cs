namespace TravelWeb.Models
{
    public class FlightSearchModel
    {
        public string Depart { get; set; }
        public string Arrival { get; set; }
        public string Trip { get; set; }

        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }

        public int Adult { get; set; }
        public int Child { get; set; }
        public int Infant { get; set; }

        public string Cabin { get; set; }
    }
}
