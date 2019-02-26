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
    public class PhController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/ph/add")]
        public HttpResponseMessage AddPh()
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
            string Loc = Convert.ToString(o["Loc"]);
            string Ph = Convert.ToString(o["Ph"]);
            string Area = Convert.ToString(o["Area"]);
            string Ext = Convert.ToString(o["Ext"]);
            string Cntry = Convert.ToString(o["Cntry"]);
            string Primary = Convert.ToString(o["Primary"]);

            Ph = Ph.ToLower().Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
            Area = Area.ToLower().Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");
            Ext = Ext.ToLower().Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            string SrchPh = "";
            string FormattedPh = "";

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else if (string.IsNullOrEmpty(Ph))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Ph] not provided.");
                }
                else if (string.IsNullOrEmpty(Loc))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Loc] not provided.");
                }
                else
                {
                    if (!string.IsNullOrEmpty(Area))
                    {
                        SrchPh = Area;
                        FormattedPh = "(" + Area + ") ";
                    }

                    SrchPh += Ph;
                    FormattedPh += Ph;

                    if (!string.IsNullOrEmpty(Ext))
                    {
                        FormattedPh = " " + Ext;
                    }

                    r = AddPhRec(CustId, Ph, Loc, Primary, Area, Ext, Cntry, SrchPh, FormattedPh);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust ComAddPh", "SQL Error", r);
                    }
                    else
                    {
                        jb.Add("Status", "Success");
                        jb.Add("PhId", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust ComAddPh", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        protected string AddPhRec(string CustId, string Ph, string Loc, string Primary,
            string Area, string Ext, string Cntry, string SrchPh, string FormattedPh)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.PhAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@Ph", Ph);
            cmd.Parameters.AddWithValue("@Loc", Loc);
            cmd.Parameters.AddWithValue("@Primary", Primary);
            cmd.Parameters.AddWithValue("@SrchPh", SrchPh);
            cmd.Parameters.AddWithValue("@FormattedPh", FormattedPh);
            cmd.Parameters.AddWithValue("@Cntry", Cntry);

            if (!string.IsNullOrEmpty(Area))
            {
                cmd.Parameters.AddWithValue("@Area", Area);
            }

            if (!string.IsNullOrEmpty(Ext))
            {
                cmd.Parameters.AddWithValue("@Ext", Ext);
            }

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}