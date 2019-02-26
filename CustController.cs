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
    public class CustController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/dupchk")]
        public HttpResponseMessage DupChk()
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
            string Username = Convert.ToString(o["Username"]);
            string Email = Convert.ToString(o["Email"]);
            string FirstName = Convert.ToString(o["FirstName"]);
            string LastName = Convert.ToString(o["LastName"]);
            string Zip = Convert.ToString(o["Zip"]);
            string Cntry = Convert.ToString(o["Cntry"]);

            DataTable TblCust = new DataTable();
            TblCust.Columns.Add("CustId", typeof(string));
            TblCust.Columns.Add("MatchType", typeof(string));
            TblCust.Columns.Add("MatchData", typeof(string));

            try
            {
                if (!string.IsNullOrEmpty(Email))
                {
                    DataTable Tbl = AppFtn.CustByEmailRec(Email);

                    foreach (DataRow dr in Tbl.Rows)
                    {
                        DataRow ro = TblCust.NewRow();
                        ro["CustId"] = dr["CustId"];
                        ro["MatchType"] = "Email";
                        ro["MatchData"] = dr["Email"];
                        TblCust.Rows.Add(ro);
                    }
                }

                if (!string.IsNullOrEmpty(Zip))
                {
                    DataTable Tbl = AppFtn.CustByAddrRec(Cntry, Zip, FirstName, LastName);

                    foreach (DataRow dr in Tbl.Rows)
                    {
                        DataRow ro = TblCust.NewRow();
                        ro["CustId"] = dr["CustId"];
                        ro["MatchType"] = "Addr";
                        ro["MatchData"] = dr["Addr"]; ;
                        TblCust.Rows.Add(ro);
                    }
                }

                if(TblCust.Rows.Count > 0)
                {
                    j = JsonConvert.SerializeObject(TblCust);
                    h.Content = new StringContent(j, Encoding.UTF8, "application/json");
                    return h;
                }
                else
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "No records returned.");                   
                }
            }
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust DupChk", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/get")]
        public HttpResponseMessage Get()
        {
            HttpResponseMessage h = new HttpResponseMessage();
            h.Headers.Add("Access-Control-Allow-Origin", "*");
            string j = "";
            string CustId = "";
            var jb = new Dictionary<string, string>();

            string result = Request.Content.ReadAsStringAsync().Result;

            JObject o = JObject.Parse(result);

            CustId = Convert.ToString(o["CustId"]);

            DataTable Tbl = new DataTable();

            try
            {
                if(!string.IsNullOrEmpty(CustId))
                {
                    Tbl = CustGetRec(CustId);
                }
                else
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required paramater [CustId] not provided.");
                }                

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
            catch (Exception x)
            {
                jb.Add("Status", "Err");
                jb.Add("CustId", CustId);
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust Get", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/add")]
        public HttpResponseMessage Add()
        {
            HttpResponseMessage h = new HttpResponseMessage();
            h.Headers.Add("Access-Control-Allow-Origin", "*");
            string r = "";
            string j = "";
            string CustId = "";
            var jb = new Dictionary<string, string>();

            string result = Request.Content.ReadAsStringAsync().Result;

            JObject o = JObject.Parse(result);
            
            
            string Email = Convert.ToString(o["Email"]);
            string FirstName = Convert.ToString(o["FirstName"]);
            string LastName = Convert.ToString(o["LastName"]);
            string MiddleName = Convert.ToString(o["MiddleName"]);
            string Prefix = Convert.ToString(o["Prefix"]);
            string Suffix = Convert.ToString(o["Suffix"]);
            string Nickname = Convert.ToString(o["Nickname"]);
            string Gender = Convert.ToString(o["Gender"]);
            string Ethnicity = Convert.ToString(o["Ethnicity"]);
            string Dob = Convert.ToString(o["Dob"]);
            string JobFtn = Convert.ToString(o["JobFtn"]);
            string Title = Convert.ToString(o["Title"]);

            string LblName = "";
            string SrchName = "";
            string Formal = "";
            string Informal = "";
            string LastFirst = "";
            FirstName = FirstName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            LastName = LastName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            MiddleName = MiddleName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Nickname = Nickname.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Title = Title.Replace("\t", "").Replace("\n", "").Replace("\r", "");

            try
            {
                if (string.IsNullOrEmpty(FirstName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [FirstName] not provided.");
                }
                else if (string.IsNullOrEmpty(LastName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [LastName] not provided.");
                }
                else
                {
                    LastFirst = LastName + ", " + FirstName;
                    SrchName = LastName + ";" + FirstName;

                    Informal = FirstName;
                    Formal = Prefix;

                    if (!string.IsNullOrEmpty(Prefix))
                    {
                        LblName = Prefix + " " + FirstName + " " + LastName;
                        Formal = Prefix + " " + FirstName + " " + LastName;
                    }
                    else
                    {
                        LblName = FirstName + " " + LastName;
                        Formal = FirstName + " " + LastName;
                    }

                    if (!string.IsNullOrEmpty(Suffix))
                    {
                        LblName += " " + Suffix;
                        Formal += " " + Suffix;
                    }

                    r = CustAddRec(FirstName, LastName, MiddleName, Prefix, Suffix, SrchName, LblName, Nickname,
                        LastFirst, Formal, Informal, JobFtn, Gender, Ethnicity, Dob, Title, Email);

                    if(r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
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
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust Add", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/upd")]
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
            string Email = Convert.ToString(o["Email"]);
            string FirstName = Convert.ToString(o["FirstName"]);
            string LastName = Convert.ToString(o["LastName"]);
            string MiddleName = Convert.ToString(o["MiddleName"]);
            string Prefix = Convert.ToString(o["Prefix"]);
            string Suffix = Convert.ToString(o["Suffix"]);
            string Nickname = Convert.ToString(o["Nickname"]);
            string Gender = Convert.ToString(o["Gender"]);
            string Ethnicity = Convert.ToString(o["Ethnicity"]);
            string Dob = Convert.ToString(o["Dob"]);
            string JobFtn = Convert.ToString(o["JobFtn"]);
            string Title = Convert.ToString(o["Title"]);

            string LblName = "";
            string SrchName = "";
            string Formal = "";
            string Informal = "";
            string LastFirst = "";
            FirstName = FirstName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            LastName = LastName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            MiddleName = MiddleName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Nickname = Nickname.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Title = Title.Replace("\t", "").Replace("\n", "").Replace("\r", "");

            try
            {
                if (string.IsNullOrEmpty(CustId))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [CustId] not provided.");
                }
                else if (string.IsNullOrEmpty(FirstName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [FirstName] not provided.");
                }
                else if (string.IsNullOrEmpty(LastName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [LastName] not provided.");
                }
                else
                {
                    LastFirst = LastName + ", " + FirstName;
                    SrchName = LastName + ";" + FirstName;

                    Informal = FirstName;
                    Formal = Prefix;

                    if (!string.IsNullOrEmpty(Prefix))
                    {
                        LblName = Prefix + " " + FirstName + " " + LastName;
                        Formal = Prefix + " " + FirstName + " " + LastName;
                    }
                    else
                    {
                        LblName = FirstName + " " + LastName;
                        Formal = FirstName + " " + LastName;
                    }

                    if (!string.IsNullOrEmpty(Suffix))
                    {
                        LblName += " " + Suffix;
                        Formal += " " + Suffix;
                    }

                    r = CustUpdRec(FirstName, LastName, MiddleName, Prefix, Suffix, SrchName, LblName, Nickname,
                        LastFirst, Formal, Informal, JobFtn, Gender, Ethnicity, Dob, Title, Email, CustId);

                    if (r.Contains("Error"))
                    {
                        jb.Add("Status", "Err");
                        jb.Add("Msg", r);
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
                jb.Add("Msg", x.Message);
                AppFtn.Err(CustId, "API Cust Upd", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        //Begin database calls

        //protected DataTable CustByEmailRec(string Email)
        //{
        //    DataTable Tbl = new DataTable();

        //    string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(ConnStr);
        //    SqlCommand cmd = new SqlCommand("dbo.CustGetByEmail", conn);
        //    cmd.Connection = conn;
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@Email", Email);

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);

        //    conn.Open();
        //    da.Fill(Tbl);
        //    conn.Close();

        //    return Tbl;
        //}

        //protected DataTable CustByAddrRec(string Cntry, string Zip, string FirstName, string LastName)
        //{
        //    DataTable Tbl = new DataTable();

        //    string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
        //    SqlConnection conn = new SqlConnection(ConnStr);
        //    SqlCommand cmd = new SqlCommand("dbo.CustGetByAddr", conn);
        //    cmd.Connection = conn;
        //    cmd.CommandType = System.Data.CommandType.StoredProcedure;
        //    cmd.Parameters.AddWithValue("@Cntry", Cntry);
        //    cmd.Parameters.AddWithValue("@Zip", Zip);
        //    cmd.Parameters.AddWithValue("@FirstName", FirstName);
        //    cmd.Parameters.AddWithValue("@LastName", LastName);

        //    SqlDataAdapter da = new SqlDataAdapter(cmd);

        //    conn.Open();
        //    da.Fill(Tbl);
        //    conn.Close();

        //    return Tbl;
        //}

        protected DataTable CustGetRec(string CustId)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGet", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }

        protected string CustAddRec(string FirstName, string LastName, string MiddleName, string Prefix,
            string Suffix, string SrchName, string LblName, string Nickname, string LastFirst, string Formal,
            string Informal, string JobFtn, string Gender, string Ethnicity, string Dob, string Title,
            string Email)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@SrchName", SrchName);
            cmd.Parameters.AddWithValue("@LblName", LblName);
            cmd.Parameters.AddWithValue("@LastFirst", LastFirst);
            cmd.Parameters.AddWithValue("@Formal", Formal);
            cmd.Parameters.AddWithValue("@Informal", Informal);

            cmd.Parameters.AddWithValue("@Dob", Dob);

            if (!string.IsNullOrEmpty(Email))
            {
                cmd.Parameters.AddWithValue("@Email", Email);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                cmd.Parameters.AddWithValue("@Title", Title);
            }

            if (!string.IsNullOrEmpty(Suffix))
            {
                cmd.Parameters.AddWithValue("@Suffix", Suffix);
            }

            if (!string.IsNullOrEmpty(Prefix))
            {
                cmd.Parameters.AddWithValue("@Prefix", Prefix);
            }

            if (!string.IsNullOrEmpty(Nickname))
            {
                cmd.Parameters.AddWithValue("@Nickname", Nickname);
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                cmd.Parameters.AddWithValue("@MiddleName", MiddleName);
            }

            if (!string.IsNullOrEmpty(Ethnicity))
            {
                cmd.Parameters.AddWithValue("@Ethnicity", Ethnicity);
            }

            if (!string.IsNullOrEmpty(JobFtn))
            {
                cmd.Parameters.AddWithValue("@JobFtn", JobFtn);
            }

            if (!string.IsNullOrEmpty(Gender))
            {
                cmd.Parameters.AddWithValue("@Gender", Gender);
            }

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }

        protected string CustUpdRec(string FirstName, string LastName, string MiddleName, string Prefix,
            string Suffix, string SrchName, string LblName, string Nickname, string LastFirst, string Formal,
            string Informal, string JobFtn, string Gender, string Ethnicity, string Dob, string Title,
            string Email, string CustId)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustUpd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@SrchName", SrchName);
            cmd.Parameters.AddWithValue("@LblName", LblName);
            cmd.Parameters.AddWithValue("@LastFirst", LastFirst);
            cmd.Parameters.AddWithValue("@Formal", Formal);
            cmd.Parameters.AddWithValue("@Informal", Informal);

            cmd.Parameters.AddWithValue("@Dob", Dob);

            if (!string.IsNullOrEmpty(Email))
            {
                cmd.Parameters.AddWithValue("@Email", Email);
            }

            if (!string.IsNullOrEmpty(Title))
            {
                cmd.Parameters.AddWithValue("@Title", Title);
            }

            if (!string.IsNullOrEmpty(Suffix))
            {
                cmd.Parameters.AddWithValue("@Suffix", Suffix);
            }

            if (!string.IsNullOrEmpty(Prefix))
            {
                cmd.Parameters.AddWithValue("@Prefix", Prefix);
            }

            if (!string.IsNullOrEmpty(Nickname))
            {
                cmd.Parameters.AddWithValue("@Nickname", Nickname);
            }

            if (!string.IsNullOrEmpty(MiddleName))
            {
                cmd.Parameters.AddWithValue("@MiddleName", MiddleName);
            }

            if (!string.IsNullOrEmpty(Ethnicity))
            {
                cmd.Parameters.AddWithValue("@Ethnicity", Ethnicity);
            }

            if (!string.IsNullOrEmpty(JobFtn))
            {
                cmd.Parameters.AddWithValue("@JobFtn", JobFtn);
            }

            if (!string.IsNullOrEmpty(Gender))
            {
                cmd.Parameters.AddWithValue("@Gender", Gender);
            }

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}