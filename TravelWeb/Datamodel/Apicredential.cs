using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Apicredential
    {
        public int ApicredentialId { get; set; }
        public int? ConsolidatorId { get; set; }
        public string Apitype { get; set; }
        public string Apiname { get; set; }
        public string Apicode { get; set; }
        public string Apistatus { get; set; }
        public string Apiswitch { get; set; }
        public string AllCredentials { get; set; }
        public string CredentialFormat { get; set; }
        public string OriginalIpcc { get; set; }
        public string ApiuserId { get; set; }
        public string ApiuserName { get; set; }
        public string Apipass { get; set; }
        public string Apiurl { get; set; }
        public string WebsiteId { get; set; }
    }
}
