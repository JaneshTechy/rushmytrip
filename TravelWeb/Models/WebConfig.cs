using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelWeb.Models.WebConfig
{

    public class Rootobject
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Appsettings AppSettings { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
        public string MicrosoftHostingLifetime { get; set; }
    }

    public class Appsettings
    {
        public string ConnectionString { get; set; }
        public string WebSitePath { get; set; }
        public string WebApiService { get; set; }
        
        public string WebSitePathXml { get; set; }
        public string WebSiteDomain { get; set; }
    }

}
