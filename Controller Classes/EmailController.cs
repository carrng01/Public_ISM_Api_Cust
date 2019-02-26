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
    public class EmailController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/email/add")]
        public HttpResponseMessage AddEmail()
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
            string Email = Convert.ToString(o["Email"]);
            string Loc = Convert.ToString(o["Loc"]);
            string Primary = Convert.ToString(o["Primary"]);

            Email = Email.ToLower().Replace(" ", "").Replace("\t", "").Replace("\r", "").Replace("\n", "");

            DataTable Tbl = AppFtn.CustByEmailRec(Email);

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else if (string.IsNullOrEmpty(Email))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Email] not provided.");
                }
                else if (string.IsNullOrEmpty(Loc))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Loc] not provided.");
                }
                else if (Tbl.Rows.Count > 0)
                {   
                    foreach(DataRow rw in Tbl.Rows)
                    {
                        if (Convert.ToString(rw["CustId"]) != CustId)
                        {
                            String Dup = Convert.ToString(Tbl.Rows[0]["CustId"]);
                            jb.Add("Status", "Err");
                            jb.Add("Msg", "Email already linked to CustId " + Dup);
                        }
                        else
                        {
                            r = AddEmailRec(CustId, Email, Loc, Primary);

                            if (r.Contains("Error"))
                            {
                                jb.Add("Status", "Err");
                                jb.Add("Msg", r);
                                AppFtn.Err(CustId, "API Cust ComAddEmail", "SQL Error", r);
                            }
                            else
                            {
                                jb.Add("Status", "Success");
                                jb.Add("EmailId", r);
                            }
                        }
                    }
                }
                else
                {
                    r = AddEmailRec(CustId, Email, Loc, Primary);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
                        AppFtn.Err(CustId, "API Cust ComAddEmail", "SQL Error", r);
                    }
                    else
                    {
                        jb.Add("Status", "Success");
                        jb.Add("EmailId", r);
                    }
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust ComAddEmail", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        //Begin database calls

        protected string AddEmailRec(string CustId, string Email, string Loc, string Primary)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.EmailAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@Loc", Loc);
            cmd.Parameters.AddWithValue("@Primary", Primary);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}