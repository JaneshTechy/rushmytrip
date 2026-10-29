using System;
using System.Collections.Generic;

#nullable disable

namespace TravelWeb.Datamodel
{
    public partial class Product
    {
        public int SupplierIdRef { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public decimal? PriceAmount { get; set; }
        public string PriceCurrency { get; set; }
        public string UnitOfMeasure { get; set; }
        public string Moq { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Condition { get; set; }
        public string PrimaryImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ProductId { get; set; }
        public string AdditionalImageUrls { get; set; }
        public long? GroupId { get; set; }
        public bool? Active { get; set; }
    }
}
