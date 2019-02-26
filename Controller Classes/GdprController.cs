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
    public class GdprController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/gdpr/get")]
        public HttpResponseMessage GetConsent()
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

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else
                {
                    DataTable Tbl = GetConsentRec(CustId);

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
                AppFtn.Err(CustId, "API Cust GdprGetConsent", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/gdpr/add")]
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
            string Val = Convert.ToString(o["Val"]);

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
                else if (string.IsNullOrEmpty(Val))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Val] not provided.");
                }
                else
                {
                    r = AddConsentRec(CustId, Code, Val);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust GdprAddConsent", "SQL Error", r);
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
                AppFtn.Err(CustId, "API Cust GdprAddConsent", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/gdpr/upd")]
        public HttpResponseMessage UpdConsent()
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
            string ConsentId = Convert.ToString(o["ConsentId"]);
            string Val = Convert.ToString(o["Val"]);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else if (string.IsNullOrEmpty(ConsentId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [ConsentId] not provided.");
                }
                else if (string.IsNullOrEmpty(Val))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Val] not provided.");
                }
                else
                {
                    r = UpdConsentRec(CustId, ConsentId, Val);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust GdprUpdConsent", "SQL Error", r);
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
                AppFtn.Err(CustId, "API Cust GdprUpdConsent", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/gdpr/del")]
        public HttpResponseMessage DelConsent()
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
            string ConsentId = Convert.ToString(o["ConsentId"]);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [BillCustId] not provided.");
                }
                else if (string.IsNullOrEmpty(ConsentId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [ConsentId] not provided.");
                }
                else
                {
                    r = DelConsentRec(ConsentId);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust GdprDelConsent", "SQL Error", r);
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
                AppFtn.Err(CustId, "API Cust GdprDelConsent", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        //Begin database calls

        protected DataTable GetConsentRec(string CustId)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGdprGetConsent", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }

        protected string AddConsentRec(string CustId, string Code, string Val)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGdprAddConsent", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@Code", Code);
            cmd.Parameters.AddWithValue("@Val", Val);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }

        protected string UpdConsentRec(string CustId, string ConsentId, string Val)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGdprUpdConsent", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ConsentId", ConsentId);
            cmd.Parameters.AddWithValue("@Val", Val);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }

        protected string DelConsentRec(string ConsentId)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGdprDelConsent", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ConsentId", ConsentId);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}