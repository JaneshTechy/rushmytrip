using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class BusinessFullDetail
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Gstin { get; set; }
        public DateTime? GstregistrationDate { get; set; }
        public string OwnershipType { get; set; }
        public string PrimaryBusiness { get; set; }
        public string SecondaryBusiness { get; set; }
        public decimal? AnnualTurnover { get; set; }
        public string Ceoname { get; set; }
        public string Employees { get; set; }
        public string Division { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string Mobile { get; set; }
        public string Landline { get; set; }
        public string TollFree { get; set; }
        public string Fax { get; set; }
        public string CardFrontPath { get; set; }
        public string CardBackPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
