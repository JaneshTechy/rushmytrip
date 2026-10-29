using System;
using System.Collections.Generic;
using System.Xml;
using TravelWeb.Models;

public class TravelportXmlParser
{
    public static FlightsResult ParseFlightsFromXml(string xmlFilePath)
    {
        FlightsResult flightsResult = new FlightsResult();
        flightsResult.FlightsList = new List<Flights>();
        flightsResult.AirlineExtra = new List<FlightsExtra>();

        XmlDocument doc = new XmlDocument();
        doc.Load(xmlFilePath);

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("air", "http://www.travelport.com/schema/air_v50_0");

        // Currency from root node
        XmlElement root = doc.DocumentElement;
        if (root != null && root.HasAttribute("CurrencyType"))
        {
            flightsResult.FlightsSearch = new FlightsSearch();
            flightsResult.FlightsSearch.Currency = root.GetAttribute("CurrencyType");
        }

        // Loop over AirSegments
        XmlNodeList segmentNodes = doc.SelectNodes("//air:AirSegment", nsmgr);
        foreach (XmlNode segNode in segmentNodes)
        {
            Flights flight = new Flights();
            flight.Provider = "Travelport";
            flight.Outbound = new List<FlightsSegment>();
            flight.Inbound = new List<FlightsSegment>();

            Airline airline = new Airline();
            if (segNode.Attributes["Carrier"] != null)
                airline.Code = segNode.Attributes["Carrier"].Value;
            flight.Airline = airline;

            FlightsSegment segment = new FlightsSegment();
            if (segNode.Attributes["FlightNumber"] != null)
                segment.FlightNo = segNode.Attributes["FlightNumber"].Value;
            if (segNode.Attributes["DepartureTime"] != null)
                segment.DepartureDate = DateTime.Parse(segNode.Attributes["DepartureTime"].Value);
            if (segNode.Attributes["ArrivalTime"] != null)
                segment.ArrivalDate = DateTime.Parse(segNode.Attributes["ArrivalTime"].Value);
            if (segNode.Attributes["Origin"] != null)
                segment.FromAirport = new Airport { Code = segNode.Attributes["Origin"].Value };
            if (segNode.Attributes["Destination"] != null)
                segment.ToAirport = new Airport { Code = segNode.Attributes["Destination"].Value };
            if (segNode.Attributes["Equipment"] != null)
                segment.EquipmentType = segNode.Attributes["Equipment"].Value;
            if (segNode.Attributes["ETicketability"] != null)
                segment.ETicket = segNode.Attributes["ETicketability"].Value;
            if (segNode.Attributes["ChangeOfPlane"] != null)
                segment.ChangePlane = segNode.Attributes["ChangeOfPlane"].Value;
            if (segNode.Attributes["Distance"] != null)
                segment.Distance = segNode.Attributes["Distance"].Value;
            if (segNode.Attributes["FlightTime"] != null)
                segment.ElapsedTime = segNode.Attributes["FlightTime"].Value;

            // outbound vs inbound
            string group = (segNode.Attributes["Group"] != null) ? segNode.Attributes["Group"].Value : "0";
            if (group == "0")
                flight.Outbound.Add(segment);
            else
                flight.Inbound.Add(segment);

            flightsResult.FlightsList.Add(flight);
        }

        return flightsResult;
    }
}
