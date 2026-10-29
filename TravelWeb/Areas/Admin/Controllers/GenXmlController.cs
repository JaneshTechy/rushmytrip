using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TravelWeb.Models;
using TravelWeb.Utilities;

namespace TravelWeb.Areas.Admin.Controllers
{
    public class GenXmlController : Controller
    {
        private IHostingEnvironment hostingEnv;
        public static string KiwiUrl = "https://kiwicom-prod.apigee.net/v2/search?";
        public static string ApiKey = "rCthx0AAgkYZDTBejCammez21GxauGs6";
        public GenXmlController(IHostingEnvironment env)

        {

            this.hostingEnv = env;

        }

        [Route("office/genxml")]
        public IActionResult Index()
        {
           System.Data.DataTable dtx = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString,CommandType.Text, "SELECT * FROM FlightBookings").Tables[0];
            return View("Areas\\Admin\\Views\\GenXml\\Index.cshtml");

        }
      
        [Route("office/genxml/UploadFiles")]
      
        [HttpPost]
        public string UploadFiles()
        {
            string result = string.Empty;

            try

            {

                long size = 0;

                var file = Request.Form.Files;

                var filename = ContentDispositionHeaderValue

                                .Parse(file[0].ContentDisposition)

                                .FileName

                                .Trim('"');

                string FilePath = hostingEnv.WebRootPath + $@"\Xml\{ filename}";

                size += file[0].Length;

                using (FileStream fs = System.IO.File.Create(FilePath))
                {
                    file[0].CopyTo(fs);
                    fs.Flush();
                }
                result = FilePath;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            
            System.Data.DataTable dt = new System.Data.DataTable();
            string fileName = result;
            FileInfo fi = new FileInfo(fileName);
            string extension = fi.Extension;
            string connString;
            //Connection String to Excel Workbook  
            if (extension.Trim() == ".xls")
            {
                connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                dt = ConvertXSLXtoDataTable(fileName, connString);

            }
            else if (extension.Trim() == ".xlsx")
            {
                connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                dt = ConvertXSLXtoDataTable(fileName, connString);

            }
            else
            {

            }


            var Destinations = (from r in dt.AsEnumerable()
                                select r["GCode"]).Distinct().ToList();
             StringBuilder sbxMain = new StringBuilder();

            sbxMain.Append("<Destination>");
            int counter = 0;
            // var Destinations = ("ORD,SFO").Split(',');
            foreach (DataRow dest in dt.Rows)
            {
                StringBuilder sbx = new StringBuilder();
                // sbx.Append("<Destination>");
                DataRow[] drow = dt.Select("GCode='" + dest + "'");
              //  if (dest["category"].ToString() == "7")
              //{ 
                counter++;
                try
                {
                    string flightQuery = string.Empty;
                    flightQuery = "fly_from=" + dest["DCode"];
                    flightQuery += "&fly_to=" + dest["GCode"];
                    flightQuery += "&date_from=" + Convert.ToDateTime(dest["DepDate1"].ToString()).ToString("dd/MM/yyyy").Replace("-", "/");
                    flightQuery += "&date_to=" + Convert.ToDateTime(dest["DepDate2"].ToString()).ToString("dd/MM/yyyy").Replace("-", "/");


                    flightQuery += "&return_from=" + Convert.ToDateTime(dest["RetDate1"].ToString()).ToString("dd/MM/yyyy").Replace("-", "/");
                    flightQuery += "&return_to=" + Convert.ToDateTime(dest["RetDate1"].ToString()).ToString("dd/MM/yyyy").Replace("-", "/");
                    flightQuery += "&flight_type=round";


                    string cabinClass = (Convert.ToInt16(dest["CabinClass"])).ToString();

                    flightQuery += "&adults=" + 1;
                    flightQuery += "&children=" + 0;
                    flightQuery += "&infants=" + 0;
                    flightQuery += "&vehicle_type=aircraft";
                    flightQuery += "&apikey=" + ApiKey;
                    flightQuery += "&curr=GBP";
                    flightQuery += "&selected_cabins=" + (cabinClass == "4" ? "M" : cabinClass == "3" ? "W" : cabinClass == "2" ? "C" : cabinClass == "1" ? "F" : "M");
                    string postUrl = KiwiUrl + flightQuery;
                    string Datax = GetDestinationData(postUrl, dest["Category"].ToString());
                    //sbx.Append(Datax);
                    sbxMain.Append(Datax);
                }
                catch
                {


                }

          //}
                //foreach (DataRow rx in drow)
                //{

                //}


                //sbx.Append("</Destination>");

                //string FilePathx = hostingEnv.WebRootPath + $@"\Xml\city\{ dest + ".xml"}";
                //if (System.IO.File.Exists(FilePathx))
                //{
                //    System.IO.File.Delete(FilePathx);
                //}

                //System.IO.File.Create(FilePathx).Close();
                //System.IO.File.WriteAllText(FilePathx, sbx.ToString());

               // break;
              }
            sbxMain.Append("</Destination>");
            string FilePathx1 = hostingEnv.WebRootPath + $@"\Xml\city\{"AllCity.xml"}";
            if (System.IO.File.Exists(FilePathx1))
            {
                System.IO.File.Delete(FilePathx1);
            }

            System.IO.File.Create(FilePathx1).Close();
            System.IO.File.WriteAllText(FilePathx1, sbxMain.ToString());



            //  string url = "https://kiwicom-prod.apigee.net/v2/search?fly_from=LON&fly_to=BKK&date_from=14/10/2020&date_to=30/10/2020&return_from=23/10/2020&return_to=23/11/2020&flight_type=round&adults=1&children=0&infants=0&vehicle_type=aircraft&apikey=rCthx0AAgkYZDTBejCammez21GxauGs6&curr=GBP&selected_cabins=M";

            return "Destinations Uploaded Successfully";
        }

        private static String GetDestinationData(string postUrl,string cat)
        {
         
            StringBuilder sbx1 = new StringBuilder();

            try
            {
                FlightsSearch FlySearch = new FlightsSearch();
                FlightsResult FResult = GetFlightResult(postUrl, FlySearch);
                FlightsResult newFlight = new FlightsResult();
                newFlight.FlightsList = FResult.FlightsList.OrderBy(f => f.TotalCost).ToList();

                // newFlight.FlightsList = FResult.FlightsList.GroupBy(f => new { f.TotalCost, f.Airline.Code });
                int ck = 0;
                string airline = "";

                string airlinedata = "";
                foreach (Flights flt in newFlight.FlightsList)
                { if (!airlinedata.Contains(flt.Airline.Code))
                    {
                        if (ck> 6)
                        {
                            break;
                        
                        }
                        airlinedata= airlinedata+ flt.Airline.Code+",";
                       // airline = flt.Airline.Code;
                        sbx1.Append("<Flights>");
                        sbx1.Append("<Category>" + cat + "</Category>");
                        sbx1.Append("<TotalCost>" + flt.TotalCost + "</TotalCost>");
                        sbx1.Append("<DCode>" + flt.Outbound[0].FromAirport.Code + "</DCode>");
                        sbx1.Append("<GCode>" + flt.Outbound[flt.Outbound.Count - 1].ToAirport.Code + "</GCode>");
                        sbx1.Append("<ACode>" + flt.Airline.Code + "</ACode>");
                        sbx1.Append("<FClass>" + flt.Outbound[0].CabinClass + "</FClass>");
                        sbx1.Append("<FClassCode>" + flt.Outbound[0].CabinClass + "</FClassCode>");
                        sbx1.Append("<DepartDate>" + Convert.ToDateTime(flt.Outbound[0].DepartureDate) + "</DepartDate>");
                        sbx1.Append("<ReturnDate>" + Convert.ToDateTime(flt.Inbound[0].DepartureDate) + "</ReturnDate>");
                        sbx1.Append("</Flights>");
                        
                    }
                }
               

            }
            catch 
            {

            }

           
            return sbx1.ToString();
        }

        public System.Data.DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            

            OleDbConnection oledbConn = new OleDbConnection(connString);
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {

                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);

                    dt = ds.Tables[0];
                }
            }
            catch
            {
            }
            finally
            {

                oledbConn.Close();
            }

            return dt;

        }

        public static FlightsResult GetFlightResult(string url, FlightsSearch FlySearch)
        {
            //FlightsResult resultx = new FlightsResult();
            //resultx.MaxPrice = 45345;
            //return resultx;


            FlightsResult result = new FlightsResult();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            var content = string.Empty;
            try
            {
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                 request.Timeout = 100000;
                request.ReadWriteTimeout = 320000;
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        using (var sr = new StreamReader(stream))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }

                dynamic Flightdatax = JObject.Parse(content);

                FlightsSearch objFlightSearch = FlySearch;
                List<Flights> lstFlight = new List<Flights>();
                int cntflight = 0;
                //List<FlightsSegment> OutboundList1 = new List<FlightsSegment>();
                //List<FlightsSegment> InboundList1 = new List<FlightsSegment>();
                //FlightsFare ObjFlightFare1 = new FlightsFare();
                foreach (var datax in Flightdatax.data)
                {
                    cntflight++;
                    if (cntflight > 50)
                    { break; }
                    result = new FlightsResult() { Error = "", MinPrice = 0, MaxPrice = 0 };

                    string Provider = "kiwi";
                    string _FlightId = System.Guid.NewGuid().ToString();// datax.id;
                    string FlightId = System.Guid.NewGuid().ToString(); //_FlightId.Split('|')[0];
                    string SessionId = System.Guid.NewGuid().ToString(); //datax.id;
                    FlightClass objFlightClass = new FlightClass();
                    Flights objFlight = new Flights();
                    Airline objAirline = new Airline();
                    string Currency = "USD";
                    bool IsDeal = false;
                    string DeepLink = datax.deep_link;
                    int TotalTime = 0;
                    double TotalCost = Convert.ToDouble(datax.fare.adults);
                    int Stops = 0;
                    string FareRules = "";
                    string LastTicketingDate = "";

                    Baglimit objBagLimit = new Baglimit();
                    string BagsPcs = "1";
                  

                    List<FlightsSegment> OutboundList = new List<FlightsSegment>();
                    List<FlightsSegment> InboundList = new List<FlightsSegment>();
                    FlightsFare ObjFlightFare = new FlightsFare();
                    ObjFlightFare.Adult = objFlightSearch.Adult; ObjFlightFare.Child = objFlightSearch.Child; ObjFlightFare.Infant = objFlightSearch.Infant;
                    ObjFlightFare.AdultFare = datax.fare.adults; ObjFlightFare.AdultTax = 0; ObjFlightFare.AdultMarkup = 0;
                    ObjFlightFare.ChildFare = datax.fare.children; ObjFlightFare.ChildTax = 0; ObjFlightFare.ChildMarkup = 0;
                    ObjFlightFare.InfantFare = datax.fare.infants; ObjFlightFare.InfantTax = 0; ObjFlightFare.InfantMarkup = 0;

                   // ObjFlightFare.AtolCharge = 0; ObjFlightFare.CardCharge = 0; TotalCost = ObjFlightFare.AvgCost;
                   
                    foreach (var FS in datax.route)
                    {
                        //if (otb > 0 && inb > 0)
                        //{
                        //    break;
                        //}

                        FlightsSegment objFlightSegement = new FlightsSegment();
                        if (FS.@return == 0)
                        {

                            string cabinClass = FS.fare_category;

                            objFlightSegement.OptionRef = FS.id;
                            objFlightSegement.SagementRef = FS.id;
                            objFlightSegement.Airline = new Airline() { Code = FS.airline.ToString() };// Common.GetAirline(FS.airline.ToString());
                            objFlightSegement.ArrivalDate = Convert.ToDateTime(FS.local_arrival);
                            objFlightSegement.CabinClass = FS.fare_classes.ToString();
                            objFlightSegement.DepartureDate = Convert.ToDateTime(FS.local_departure);
                            objFlightSegement.FlightNo = FS.flight_no.ToString();
                            objFlightSegement.EquipmentType = FS.equipment.ToString();
                            objFlightSegement.FromAirport = new Airport() { Code = FS.flyFrom.ToString() }; //Common.GetAirport(FS.flyFrom.ToString());
                            objFlightSegement.ToAirport = new Airport() { Code = FS.flyTo.ToString() }; //Common.GetAirport(FS.flyTo.ToString());
                            objFlightSegement.TotalTime = (int.Parse(datax.duration.departure.ToString()) / 60).ToString();
                            objFlightSegement.OperatingAirline = new Airline() { Code = FS.airline.ToString() };// Common.GetAirline(FS.airline.ToString());
                            objAirline = objFlightSegement.Airline;
                            objFlightSegement.CabinClass = (cabinClass == "M" ? "Economy" : cabinClass == "W" ? "Economy Premium" : cabinClass == "C" ? "Business" : cabinClass == "F" ? "First Class" : "Economy");
                            objFlightSegement.BaggageAllowance = BagsPcs;

                            OutboundList.Add(objFlightSegement);

                        }
                        else
                        {

                            string cabinClass = FS.fare_category;

                            objFlightSegement.OptionRef = FS.id;
                            objFlightSegement.SagementRef = FS.id;
                            objFlightSegement.Airline = new Airline() { Code = FS.airline.ToString() };// Common.GetAirline(FS.airline.ToString());
                            objFlightSegement.ArrivalDate = Convert.ToDateTime(FS.local_arrival);
                            objFlightSegement.CabinClass = FS.fare_classes.ToString();
                            objFlightSegement.DepartureDate = Convert.ToDateTime(FS.local_departure);
                            objFlightSegement.FlightNo = FS.flight_no.ToString();
                            objFlightSegement.EquipmentType = FS.equipment.ToString();
                            objFlightSegement.FromAirport = new Airport() { Code = FS.flyFrom.ToString() }; //Common.GetAirport(FS.flyFrom.ToString());
                            objFlightSegement.ToAirport = new Airport() { Code = FS.flyTo.ToString() };// Common.GetAirport(FS.flyTo.ToString()); ;
                            objFlightSegement.TotalTime = (int.Parse(datax.duration.@return.ToString()) / 60).ToString();
                            objFlightSegement.OperatingAirline = new Airline() { Code = FS.airline.ToString() };// Common.GetAirline(FS.airline.ToString());
                            objAirline = objFlightSegement.Airline;
                            objFlightSegement.CabinClass = (cabinClass == "M" ? "Economy" : cabinClass == "W" ? "Economy Premium" : cabinClass == "C" ? "Business" : cabinClass == "F" ? "First Class" : "Economy");
                            objFlightSegement.BaggageAllowance = BagsPcs;

                            InboundList.Add(objFlightSegement);

                        }


                    }

                    if (OutboundList.Count == 1)
                    {
                        Stops = 0;
                    }
                    if (OutboundList.Count == 2)
                    {
                        Stops = 1;
                    }
                    if (OutboundList.Count == 3)
                    {
                        Stops = 2;
                    }

                    //Bag Added by praveen                                    

                    objBagLimit.hand_width = (datax.baglimit.hand_width != null ? datax.baglimit.hand_width.ToString() : "22");
                    objBagLimit.hand_height = (datax.baglimit.hand_height != null ? datax.baglimit.hand_height.ToString() : "35");
                    objBagLimit.hand_length = (datax.baglimit.hand_length != null ? datax.baglimit.hand_length.ToString() : "55");
                    objBagLimit.hand_weight = (datax.baglimit.hand_weight != null ? datax.baglimit.hand_weight.ToString() : "7");

                    try
                    {
                        objBagLimit.hold_width = (datax.baglimit.hold_width != null ? datax.baglimit.hold_width.ToString() : "0");
                        objBagLimit.hold_height = (datax.baglimit.hold_height != null ? datax.baglimit.hold_height.ToString() : "0");
                        objBagLimit.hold_length = (datax.baglimit.hold_length != null ? datax.baglimit.hold_length.ToString() : "0");
                        objBagLimit.hold_dimensions_sum = (datax.baglimit.hold_dimensions_sum != null ? datax.baglimit.hold_dimensions_sum.ToString() : "0");
                        objBagLimit.hold_weight = (datax.baglimit.hold_weight != null ? datax.baglimit.hold_weight.ToString() : "0");
                    }
                    catch (Exception)
                    {
                        objBagLimit.hold_width = "0";
                        objBagLimit.hold_height = "0";
                        objBagLimit.hold_length = "0";
                        objBagLimit.hold_weight = "0";
                    }

                    objFlight.DeepLink = DeepLink;
                    objFlight.Provider = Provider;
                    objFlight.FlightId = FlightId;
                    objFlight.SessionId = SessionId;
                    objFlight.FlightClass = objFlightClass;
                    objFlight.Airline = objAirline;
                    objFlight.Currency = Currency;
                    objFlight.IsDeal = IsDeal;
                    objFlight.DeepLink = DeepLink;
                    objFlight.TotalTime = datax.duration.total;
                    objFlight.TotalCost = TotalCost;
                    objFlight.Stops = Stops;
                    objFlight.FareRules = FareRules;
                   // objFlight.LastTicketingDate = LastTicketingDate;
                    objFlight.FlightFare = ObjFlightFare;
                    objFlight.Outbound = OutboundList;
                    objFlight.Inbound = InboundList;
                    objFlight.Baglimit = objBagLimit;
                   // objFlight.ListBagsPrice = lstBagsPrice;

                    lstFlight.Add(objFlight);
                }

                result.FlightsSearch = FlySearch;
                result.FlightsList = lstFlight;

            }
            catch (Exception ex)
            {
                
            }


            return result;
        }
    }
    public class FlightData
    {
        public string DCode { get; set; }
        public string GCode { get; set; }
        public string ACode { get; set; }
        public string FClass { get; set; }
        public string DeaprtDate { get; set; }
        public string ReturnDate { get; set; }

    }
  
}  

