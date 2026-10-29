using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace TravelWeb.Controllers
{

    public class AirlineController : Controller
    {
        private IHostingEnvironment hostingEnv;
        public static string KiwiUrl = "https://kiwicom-prod.apigee.net/v2/search?";
        public AirlineController(IHostingEnvironment env)

        {

            this.hostingEnv = env;

        }
        public IActionResult Index()
        {

            //string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{"allcity.xml"}";
            string FilePath = "http://doket.net/xmls/city/shivatravel/allcity.xml";

            DataSet dsx = new DataSet();
            dsx.ReadXml(FilePath);

            DataTable dtx = new DataTable();

            dtx = dsx.Tables[0].Clone();
            dtx.Columns.Add("TotalFare", typeof(int));

            var query = from row in dsx.Tables[0].AsEnumerable()
                        group row by row.Field<string>("ACode") into grp
                        select new
                        {
                            Id = grp.Key,
                            sum = grp.Min(r => r.Field<string>("TotalCost"))
                        };



            DataRow dr;
            foreach (var grp in query)
            {
                if (Convert.ToInt32(grp.sum) > 0)
                {

                    dr = dtx.NewRow();
                    dr["ACode"] = grp.Id;
                    dr["TotalFare"] = Convert.ToInt32(grp.sum);
                    dtx.Rows.Add(dr);
                    dtx.AcceptChanges();
                }

            }

            DataRow[] drvx = dtx.Select("", "TotalFare asc");
            DataTable fnldt = new DataTable();
            fnldt = dtx.Clone();

            foreach (DataRow cc in drvx)
            {
                fnldt.ImportRow(cc);

                fnldt.AcceptChanges();
            }
            return View(fnldt);



        }
        [Route("airlines/{Acode}")]
        public IActionResult airline(string Acode)
        {

            // string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{"allcity.xml"}";
            string FilePath = "http://doket.net/xmls/city/shivatravel/allcity.xml";

            DataSet dsx = new DataSet();
            dsx.ReadXml(FilePath);

            DataTable dtx = new DataTable();

            dtx = dsx.Tables[0].Clone();
            dtx.Columns.Add("TotalFare", typeof(int));

            DataRow[] drxc = dsx.Tables[0].Select("ACode='" + Acode + "'");


            DataRow dr;
            foreach (var grp in drxc)
            {
                dr = dtx.NewRow();
                dr["ACode"] = grp["ACode"];
                dr["DCode"] = grp["DCode"];
                dr["GCode"] = grp["GCode"];
                dr["FClass"] = grp["FClass"];
                dr["TotalFare"] = grp["TotalCost"];
                dr["DepartDate"] = grp["DepartDate"];
                dr["ReturnDate"] = grp["ReturnDate"];
                // dr["TotalFare"] = Convert.ToInt32(grp.TotalFare);
                dtx.Rows.Add(dr);
                dtx.AcceptChanges();
            }
            DataRow[] drvx = dtx.Select("", "TotalFare asc");
            DataTable fnldt = new DataTable();
            fnldt = dtx.Clone();

            foreach (DataRow cc in drvx)
            {
                fnldt.ImportRow(cc);

                fnldt.AcceptChanges();
            }
            return View(fnldt);


        }
    }
}
