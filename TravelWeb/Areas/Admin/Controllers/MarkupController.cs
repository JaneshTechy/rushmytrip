using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Data.OleDb;
using TravelWeb.Admin.Models;
using TravelWeb.Utilities;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using System.Data.OleDb;
using SqlData.Data;

namespace TravelWeb.Admin.Controllers
{
   
    public class MarkupController : Controller
    {
        private IHostingEnvironment hostingEnv;
        public MarkupController(IHostingEnvironment env)

        {

            this.hostingEnv = env;

        }
        // GET: Markup
        public ActionResult Index()
        {
            

            List<FareMarkUp> fareMarkUps = new List<FareMarkUp>();           
            string itemQuery = "SELECT  top 100 * FROM FareMarkUp_new ORDER BY Origin";          
            DataSet ds = SqlData.Data.SqlHelperData.ExecuteDataset(Common.ConnectionString,CommandType.Text,itemQuery);

            if (ds != null)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    fareMarkUps.Add(new FareMarkUp()
                    {
                        //Id = Convert.ToInt32(dr["Id"].ToString()),
                        Origin = dr["Origin"].ToString(),
                        Destination = dr["Destination"].ToString(),
                        Airlines = dr["Airlines"].ToString(),
                        MarkUpType = dr["MarkUpType"].ToString(),
                        MarkUp = Convert.ToDouble(dr["MarkUp"].ToString()),
                        Class = dr["Classes"].ToString(),
                        StartDate = dr["StartDate"].ToString(),
                        EndDate = dr["EndDate"].ToString(),
                        EffectiveDate = dr["EffectiveDate"].ToString(),
                        PaxType = dr["PaxType"].ToString(),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return View("Areas\\Admin\\Views\\markup\\Index.cshtml", fareMarkUps);
           
        }

        public ActionResult Add()
        {
            return View();
        }

     
        public ActionResult ImportFareMarkUp()
        {
            return View("Areas\\Admin\\Views\\markup\\ImportFareMarkUp.cshtml");
        }

        [Route("markup/UploadImportFareMarkUp")]

        [HttpPost]
        public string UploadImportFareMarkUp()
        {
          
            string result = string.Empty;
            string FilePath = "";
            try

            {

                long size = 0;

                var file = Request.Form.Files;

                var filename = ContentDispositionHeaderValue

                                .Parse(file[0].ContentDisposition)

                                .FileName

                                .Trim('"');

                 FilePath = hostingEnv.WebRootPath + $@"\Upload\Markup\{ filename}";

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


            System.IO.File.Delete(FilePath);
            SqlParameter[] commandParameters2 = new SqlParameter[2];

            commandParameters2[0] = new SqlParameter("@dtx", SqlDbType.Structured) { Value = dt };
            commandParameters2[1] = new SqlParameter("@ModifiedBy", SqlDbType.VarChar) { Value = "Admin" };

            DataTable dx = SqlHelperData.ExecuteDataset(Common.ConnectionString, CommandType.StoredProcedure, "FOSP_ImportFareMarkupType", commandParameters2).Tables[0];
            return dx.Rows.Count + " Fare Uploaded Successfully!";



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
            catch (Exception ex)
            {
            }
            finally
            {

                oledbConn.Close();
            }

            return dt;

        }

       

        public ActionResult Status(string Id)
        {
            return View();
        }

        /// <summary>
        /// Export data
        /// </summary>
        /// <returns></returns>
        //public ActionResult ExportData()
        //{
        //    DataTable dt = SqlHelperData.ExecuteDataset(SqlHelperData.ConnectionString,CommandType.Text, "SELECT [Origin] ,[Destination],[Airlines],[MarkUpType],[MarkUp],[Classes] ,CONVERT(char(10),[StartDate],126)[StartDate],CONVERT(char(10),[EndDate],126)[EndDate],CONVERT(char(10),[EffectiveDate],126)[EffectiveDate],[PaxType],[Notes]  FROM FareMarkUp_NEW").Tables[0];//your datatable
        //    string attachment = "attachment; filename=FareMarkUp.xls";
        //    Response.ClearContent();
        //    Response.AddHeader("content-disposition", attachment);
        //    Response.ContentType = "application/vnd.ms-excel";
        //    string tab = "";
        //    foreach (DataColumn dc in dt.Columns)
        //    {
        //        Response.Write(tab + dc.ColumnName);
        //        tab = "\t";
        //    }
        //    Response.Write("\n");
        //    int i;
        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        tab = "";
        //        for (i = 0; i < dt.Columns.Count; i++)
        //        {
        //            Response.Write(tab + dr[i].ToString());
        //            tab = "\t";
        //        }
        //        Response.Write("\n");
        //    }
        //    Response.End();
        //    return Redirect("/markup/?msg=6");


        //    //var grid = new GridView();

        //    //List<FareMarkUp> fareMarkUps = new List<FareMarkUp>();
        //    //string itemQuery = "SELECT * FROM FareMarkUp ORDER BY Origin";
        //    //DataSet ds = SqlHelper.ExecuteDataset(itemQuery, null);

        //    //if (ds != null && ds.Tables[0].Rows.Count > 0)
        //    //{
        //    //    //Table declaration
        //    //    DataTable dt = new DataTable();
        //    //    //create column for export data
        //    //    Common.AddColumnsToTable("SNo.;Origin;Destination;Airlines;Class;Markup Type;Markup;Status", dt);
        //    //    //Add header row
        //    //    DataRow drHeader = dt.NewRow();
        //    //    List<string> columnNames = new List<string>();
        //    //    foreach (DataColumn column in dt.Columns)
        //    //    {
        //    //        columnNames.Add(column.ColumnName);
        //    //    }
        //    //    drHeader.ItemArray = columnNames.ToArray();
        //    //    //Add row at index 2
        //    //    dt.Rows.Add(drHeader);
        //    //    //Bind data row
        //    //    int SNo = 0;
        //    //    foreach (DataRow item in ds.Tables[0].Rows)
        //    //    {
        //    //        DataRow dr = dt.NewRow();
        //    //        dr["SNo."] = (++SNo);
        //    //        dr["Origin"] = item["Origin"];
        //    //        dr["Destination"] = item["Destination"];
        //    //        dr["Airlines"] = item["Airlines"];
        //    //        dr["Class"] = item["Classes"];
        //    //        dr["Markup Type"] = item["MarkupType"];
        //    //        dr["Markup"] = item["Markup"];                   
        //    //        dr["Status"] = item["Status"].ToString().ToLower().Equals("true") ? "Active" : "InActive";
        //    //        dt.Rows.Add(dr);
        //    //    }
        //    //    //set datasource
        //    //    grid.DataSource = dt;
        //    //}

        //    //string fileName = "FareMarkup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xls";
        //    ////hide grid view header
        //    //grid.ShowHeader = false;
        //    ////bind grid                    
        //    //grid.DataBind();                
        //    //Response.ClearContent();
        //    //Response.Buffer = true;
        //    //Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
        //    //Response.ContentType = "application/ms-excel";
        //    //Response.Charset = "";
        //    //StringWriter objStringWriter = new StringWriter();
        //    //HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);
        //    //grid.RenderControl(objHtmlTextWriter);
        //    //Response.Output.Write(objStringWriter.ToString());
        //    //Response.Flush();
        //    //Response.End();
        //    //return Redirect("/markup/?msg=6");
        //}

        //public ActionResult Manage()
        //{
        //    if (Common.CheckLogin())
        //        return Redirect("/");

        //    if (Request.QueryString["mode"] != null)
        //        ViewBag.mode = Request.QueryString["mode"];
        //    else
        //        return Redirect("/markup/");

        //    if (Request.QueryString["id"] != null)
        //        ViewBag.id = Request.QueryString["id"];

        //    Markup MarkupList = new Markup();

        //    if (Request.QueryString["mode"].Equals("edit"))
        //    {
        //        SqlParameter[] commandParameters = new SqlParameter[1];
        //        commandParameters[0] = new SqlParameter("@Id", SqlDbType.Int) { Value = int.Parse(Request.QueryString["id"]) };

        //        DataSet ds = SqlHelper.ExecuteDataset("SELECT * FROM Markup WHERE Id=@Id", commandParameters);

        //        if (ds != null)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                MarkupList.Id = Convert.ToInt32(dr["Id"].ToString());
        //                MarkupList.SiteCode = dr["SiteCode"].ToString();
        //                MarkupList.Provider = dr["Provider"].ToString();
        //                MarkupList.FetchType = dr["FetchType"].ToString();
        //                MarkupList.AdultMarkup = Convert.ToDouble(dr["AdultMarkup"].ToString());
        //                MarkupList.ChildMarkup = Convert.ToDouble(dr["ChildMarkup"].ToString());
        //                MarkupList.InfantMarkup = Convert.ToDouble(dr["InfantMarkup"].ToString());
        //            }
        //        }
        //    }
        //    else
        //    {
        //        MarkupList.Id = 0;
        //        MarkupList.SiteCode = "";
        //        MarkupList.Provider = "";
        //    }
        //    return View(MarkupList);
        //}

        //public ActionResult Option()
        //{
        //    if (Common.CheckLogin())
        //        return Redirect("/");

        //    if (Request.Form["mode"] == null)
        //        return Redirect("/markup/");

        //    if (Request.Form["mode"].Equals("add"))
        //    {

        //        SqlParameter[] commandParameters = new SqlParameter[8];
        //        commandParameters[0] = new SqlParameter("@sitecode", SqlDbType.VarChar, 50) { Value = Request.Form["txt_sitecode"] };
        //        commandParameters[1] = new SqlParameter("@provider", SqlDbType.VarChar, 50) { Value = Request.Form["txt_provider"] };
        //        commandParameters[2] = new SqlParameter("@fetchtype", SqlDbType.VarChar, 50) { Value = Request.Form["txt_fetchtype"] };
        //        commandParameters[3] = new SqlParameter("@adultmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_adultmarkup"] };
        //        commandParameters[4] = new SqlParameter("@childmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_childmarkup"] };
        //        commandParameters[5] = new SqlParameter("@infantmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_infantmarkup"] };

        //        DataSet ds = SqlHelper.ExecuteDataset("INSERT INTO Markup(SiteCode,Provider,FetchType,AdultMarkup,ChildMarkup,InfantMarkup) VALUES(@sitecode,@provider,@fetchtype,@adultmarkup,@childmarkup,@infantmarkup) SELECT SCOPE_IDENTITY()", commandParameters);

        //        return Redirect("/markup/?msg=1");
        //    }
        //    if (Request.Form["mode"].Equals("edit") && Request.Form["id"] != null)
        //    {

        //        SqlParameter[] commandParameters = new SqlParameter[7];
        //        commandParameters[0] = new SqlParameter("@id", SqlDbType.Int) { Value = Request.Form["id"] };
        //        commandParameters[1] = new SqlParameter("@sitecode", SqlDbType.VarChar, 50) { Value = Request.Form["txt_sitecode"] };
        //        commandParameters[2] = new SqlParameter("@provider", SqlDbType.VarChar, 50) { Value = Request.Form["txt_provider"] };
        //        commandParameters[3] = new SqlParameter("@fetchtype", SqlDbType.VarChar, 50) { Value = Request.Form["txt_fetchtype"] };
        //        commandParameters[4] = new SqlParameter("@adultmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_adultmarkup"] };
        //        commandParameters[5] = new SqlParameter("@childmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_childmarkup"] };
        //        commandParameters[6] = new SqlParameter("@infantmarkup", SqlDbType.Decimal) { Value = Request.Form["txt_infantmarkup"] };

        //        DataSet ds = SqlHelper.ExecuteDataset("UPDATE Markup SET SiteCode=@sitecode,Provider=@provider,FetchType=@fetchtype,AdultMarkup=@adultmarkup,ChildMarkup=@childmarkup,InfantMarkup=@infantmarkup  WHERE Id=@id", commandParameters);

        //        return Redirect("/markup/?msg=2");
        //    }
        //    if (Request.Form["mode"].Equals("delete") && Request.Form["id"] != null)
        //    {
        //        SqlParameter[] commandParameters = new SqlParameter[1];
        //        commandParameters[0] = new SqlParameter("@id", SqlDbType.Int) { Value = int.Parse(Request.Form["id"]) };

        //        SqlHelper.ExecuteDataset("DELETE FROM Markup WHERE Id=@id", commandParameters);
        //        return Redirect("/markup/?msg=3");
        //    }
        //    return View();
        //}
    }
}