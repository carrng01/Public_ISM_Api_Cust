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
    public class CmntController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/cmnt/get")]
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
            string CmntType = Convert.ToString(o["CmntType"]);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else
                {
                    DataTable Tbl = GetCmntRec(CustId, CmntType);

                    if (Tbl.Rows.Count > 0)
                    {                       
                        j = JsonConvert.SerializeObject(Tbl);
                        jb.Add("Status", "Success");
                        jb.Add("Data", j);
                    }
                    else
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", "No records returned.");
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust Cmnt Get", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        protected DataTable GetCmntRec(string CustId, string CmntType)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustCmntGet", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);

            if(!string.IsNullOrEmpty(CmntType))
            {
                cmd.Parameters.AddWithValue("@CmntType", CmntType);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            
            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }
    }
}