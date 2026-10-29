using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TravelWeb.Models;
using TravelWeb.Utilities;

namespace TravelWeb.Controllers
{
    public class CityController : Controller
    {
        private IHostingEnvironment hostingEnv;
        public static string KiwiUrl = "https://kiwicom-prod.apigee.net/v2/search?";
        public static string ApiKey = "rCthx0AAgkYZDTBejCammez21GxauGs6";
        public CityController(IHostingEnvironment env)

        {

            this.hostingEnv = env;

        }
        [Route("offer/{dest}")]
        public IActionResult Index(string dest)
        {

            //    string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{ dest + ".xml"}";
            //    DataSet dsx = new DataSet();
            //    dsx.ReadXml(FilePath);
            //    DataTable dtx = new DataTable();

            //dtx=    dsx.Tables[0].Clone();
            //    dtx.Columns.Add("TotalFare", typeof(int));


            // string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{"allcity.xml"}";
            string FilePath = "http://doket.net/xmls/city/shivatravel/allcity.xml";

            DataSet dsx = new DataSet();
            dsx.ReadXml(FilePath);

            DataTable dtx = new DataTable();

            dtx = dsx.Tables[0].Clone();
            dtx.Columns.Add("TotalFare", typeof(int));

            DataRow[] drxc = dsx.Tables[0].Select("GCode='" + dest + "'");

            int cnt = 0;


            DataRow drv;
            foreach (DataRow drc in drxc)
            {

                try
                {
                    drv = dtx.NewRow();
                    drv["CreatedDate"] = drc["CreatedDate"];
                    drv["DCode"] = drc["DCode"].ToString();
                    drv["GCode"] = dest;// drc[2].ToString();

                    drv["ACode"] = drc["ACode"].ToString();
                    drv["FClass"] = drc["FClass"].ToString();

                    drv["FClassCode"] = drc["FClassCode"].ToString();
                    drv["DepartDate"] = drc["DepartDate"].ToString();

                    drv["ReturnDate"] = drc["ReturnDate"].ToString();
                    drv["TotalFare"] = Convert.ToInt32(drc["TotalCost"]) + 1;
                    dtx.Rows.Add(drv);
                    dtx.AcceptChanges();
                    cnt++;
                }
                catch (Exception ex)
                {


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

        [Route("offer/{offer}/{sn}")]
        public IActionResult Offers(string offer, int sn)
        {
            ViewBag.Pagetitle = offer.Replace("-", " ");
            // string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{"allcity.xml"}";
            string FilePath = "http://doket.net/xmls/city/shivatravel/allcity.xml";

            DataSet dsx = new DataSet();
            dsx.ReadXml(FilePath);

            DataTable dtx = new DataTable();

            dtx = dsx.Tables[0].Clone();
            dtx.Columns.Add("TotalFare", typeof(int));

            DataRow[] drxc = dsx.Tables[0].Select("Category='" + sn + "'");


            DataRow dr;
            foreach (var grp in drxc)
            {
                dr = dtx.NewRow();
                dr["CreatedDate"] = grp["CreatedDate"];
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


        [Route("popular/city")]
        public PartialViewResult popularCity(string dest)
        {

            // string FilePath = hostingEnv.WebRootPath + $@"\Xml\city\{"allcity.xml"}";
            //string FilePath = "http://doket.net/xmls/city/shivatravel/allcity.xml";
            //DataSet dsx = new DataSet();
            //dsx.ReadXml(FilePath);

            //DataTable dtx = new DataTable();

            //dtx = dsx.Tables[0].Clone();
            //dtx.Columns.Add("TotalFare", typeof(int));

            //DataRow[] drxc = dsx.Tables[0].Select("GCode='" + dest + "'");

            //int cnt = 0;


            //DataRow drv;
            //foreach (DataRow drc in drxc)
            //{

            //    try
            //    {
            //        drv = dtx.NewRow();
            //        drv["CreatedDate"] = drc["CreatedDate"];
            //        drv["DCode"] = drc["DCode"].ToString();
            //        drv["GCode"] = dest;// drc[2].ToString();

            //        drv["ACode"] = drc["ACode"].ToString();
            //        drv["FClass"] = drc["FClass"].ToString();

            //        drv["FClassCode"] = drc["FClassCode"].ToString();
            //        drv["DepartDate"] = drc["DepartDate"].ToString();

            //        drv["ReturnDate"] = drc["ReturnDate"].ToString();
            //        drv["TotalFare"] = Convert.ToInt32(drc["TotalCost"]) + 1;
            //        dtx.Rows.Add(drv);
            //        dtx.AcceptChanges();
            //        cnt++;
            //    }
            //    catch (Exception ex)
            //    {


            //    }
            //}

            //DataRow[] drvx = dtx.Select("", "TotalFare asc");
            DataTable fnldt = new DataTable();
            //fnldt = dtx.Clone();

            //foreach (DataRow cc in drvx)
            //{
            //    fnldt.ImportRow(cc);

            //    fnldt.AcceptChanges();
            //}


            //return View(fnldt);
            return PartialView("_popularCity", fnldt);
        }

    }
}
