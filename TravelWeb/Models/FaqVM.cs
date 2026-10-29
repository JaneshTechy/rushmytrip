namespace TravelWeb.Models
{
    public class FaqVM
    {
        public int? FaqId { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public int SortOrder { get; set; }
    }
}
