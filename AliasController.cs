using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Cust.Controllers
{
    public class AliasController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/alias/add")]
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
                else if (string.IsNullOrEmpty(SrchName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [SrchName] not provided.");
                }
                else
                {
                    r = AddAliasRec(CustId, Code, SrchName);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust AddAlias", "SQL Error", r);
                    }
                    else
                    {
                        jb.Add("Status", "Success");
                        jb.Add("AliasId", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust AddAlias", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/alias/upd")]
        public HttpResponseMessage Upd()
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
                else if (string.IsNullOrEmpty(SrchName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [SrchName] not provided.");
                }
                else
                {
                    r = UpdAliasRec(CustId, Code, SrchName);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust Alias Upd", "SQL Error", r);
                    }
                    else
                    {
                        jb.Add("Status", "Success");
                        jb.Add("Msg", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust Alias Upd", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        //Begin database calls

        protected string AddAliasRec(string CustId, string Code, string SrchName)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.AliasAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@SrchName", SrchName);

            if(!string.IsNullOrEmpty(Code))
            {
                cmd.Parameters.AddWithValue("@Code", Code);
            }          

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }

        protected string UpdAliasRec(string CustId, string Code, string SrchName)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.AliasUpd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@SrchName", SrchName);

            cmd.Parameters.AddWithValue("@Code", Code);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}