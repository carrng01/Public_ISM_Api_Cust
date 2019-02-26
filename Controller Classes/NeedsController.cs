using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Cust.Controllers
{
    public class NeedsController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/need/get")]
        public HttpResponseMessage AddAlias()
        {
            HttpResponseMessage h = new HttpResponseMessage();
            h.Headers.Add("Access-Control-Allow-Origin", "*");
            string r = "";
            string j = "";
            string CustId = "";
            var jb = new Dictionary<string, string>();

            string result = Request.Content.ReadAsStringAsync().Result;

            JObject o = JObject.Parse(result);

            CustId = Convert.ToString(o["CustId"]);
            string Code = Convert.ToString(o["Code"]);
            string SrchName = Convert.ToString(o["SrchName"]);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else
                {
                    DataTable Tbl = GetNeedRec(CustId);

                    if (Tbl.Rows.Count == 0)
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", "No records returned.");
                    }
                    else
                    {
                        r = JsonConvert.SerializeObject(Tbl);
                        jb.Add("Status", "Success");
                        jb.Add("Data", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust GetNeeds", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/need/add")]
        public HttpResponseMessage AddConsent()
        {
            HttpResponseMessage h = new HttpResponseMessage();
            h.Headers.Add("Access-Control-Allow-Origin", "*");
            string r = "";
            string j = "";
            string CustId = "";
            var jb = new Dictionary<string, string>();

            string result = Request.Content.ReadAsStringAsync().Result;

            JObject o = JObject.Parse(result);

            CustId = Convert.ToString(o["CustId"]);
            string Code = Convert.ToString(o["Code"]);
            string Type = Convert.ToString(o["Type"]);
            string Cmnt = Convert.ToString(o["Cmnt"]);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else if (string.IsNullOrEmpty(Code))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Code] not provided.");
                }
                else if (string.IsNullOrEmpty(Type))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Type] not provided.");
                }
                else
                {
                    r = AddNeedRec(CustId, Code, Type, Cmnt);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust AddNeed", "SQL Error", r);
                    }
                    else
                    {
                        jb.Add("Status", "Success");
                        jb.Add("ConsentId", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust AddNeed", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        protected DataTable GetNeedRec(string CustId)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustNeedGet", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }

        protected string AddNeedRec(string CustId, string Code, string Type, string Cmnt)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustNeedAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@Type", Type);
            cmd.Parameters.AddWithValue("@Cmnt", Cmnt);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}