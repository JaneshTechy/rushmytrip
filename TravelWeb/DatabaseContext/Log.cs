using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.DatabaseContext
{
    public partial class Log
    {
        public int LogId { get; set; }
        public string Level { get; set; }
        public string CallSite { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string AdditionalInfo { get; set; }
        public DateTime? LoggedOnDate { get; set; }
    }
}
