using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelWeb.Models
{
    public enum FlightWay : int
    {
        None = 0,
        OneWay = 1,
        RoundTrip = 2
    }

    public enum FlightClass : int
    {
        None = 0,
        First = 1,
        Business = 2,
        EconomyPremium = 3,
        Economy = 4
    }

    public enum PassengerType : int
    {
        None = 0,
        Adult = 1,
        Child = 2,
        Infant = 3
    }

    public enum Gender : int
    {
        None = 0,
        Male = 1,
        Female = 2,
    }

    public enum SiteCode : int
    {
        ALL = 1,
        FLIGHTOFFICE = 2,
        VIMAN = 3,
        TRAVOLIE = 4
    }

    public enum FlightSearchSource
    {
        FS,
        Trabber,
        FC
    }

}