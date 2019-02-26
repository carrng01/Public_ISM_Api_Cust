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
    public class AddrController : ApiController
    {
        [System.Web.Http.AcceptVerbs("GET", "POST")]
        [Route("~/cust/addr/add")]
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

            CustId = Convert.ToString(o["CustId"]);
            string LblName = Convert.ToString(o["LblName"]);
            string Title = Convert.ToString(o["Title"]);
            string Comp = Convert.ToString(o["Comp"]);
            string AddrType = Convert.ToString(o["AddrType"]);

            string Addr1 = Convert.ToString(o["Addr1"]);
            string Addr2 = Convert.ToString(o["Addr2"]);
            string Addr3 = Convert.ToString(o["Addr3"]);
            string Addr4 = Convert.ToString(o["Addr4"]);

            string City = Convert.ToString(o["City"]);
            string St = Convert.ToString(o["St"]);
            string Zip = Convert.ToString(o["Zip"]);
            string Cntry = Convert.ToString(o["Cntry"]);

            string AddrHdr = "";
            string CityLine = "";
            string FormattedAddr = "";

            LblName = LblName.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Title = Title.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Comp = Comp.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Addr1 = Addr1.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Addr2 = Addr2.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Addr3 = Addr3.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Addr4 = Addr4.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            City = City.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            St = St.Replace("\t", "").Replace("\n", "").Replace("\r", "");
            Zip = Zip.Replace("\t", "").Replace("\n", "").Replace("\r", "");

            try
            {
                if (string.IsNullOrEmpty(LblName))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [LblName] not provided.");
                }
                else if (string.IsNullOrEmpty(Addr1))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Addr1] not provided.");
                }
                else if (string.IsNullOrEmpty(Zip))
                {
                    jb.Add("Status", "Err");
                    jb.Add("Msg", "Required parameter [Zip] not provided.");
                }
                else
                {
                    AddrHdr = LblName;
                    AddrHdr += System.Environment.NewLine;

                    if (!string.IsNullOrEmpty(Title))
                    {
                        
                        AddrHdr += Title;
                        AddrHdr += System.Environment.NewLine;
                    }

                    if(!string.IsNullOrEmpty(Comp))
                    {
                        AddrHdr += Comp;
                        AddrHdr += System.Environment.NewLine;
                    }

                    if(!string.IsNullOrEmpty(City))
                    {
                        CityLine = City + ", ";
                    }

                    if (!string.IsNullOrEmpty(St))
                    {
                        CityLine += St + " ";
                    }

                    CityLine += Zip;

                    FormattedAddr = Addr1;
                    FormattedAddr += System.Environment.NewLine;

                    if(!string.IsNullOrEmpty(Addr2))
                    {
                        FormattedAddr = Addr2;
                        FormattedAddr += System.Environment.NewLine;
                    }

                    if (!string.IsNullOrEmpty(Addr3))
                    {
                        FormattedAddr = Addr3;
                        FormattedAddr += System.Environment.NewLine;
                    }

                    if (!string.IsNullOrEmpty(Addr4))
                    {
                        FormattedAddr = Addr4;
                        FormattedAddr += System.Environment.NewLine;
                    }

                    FormattedAddr += CityLine;

                    r = AddAddrRec(LblName, Title, Comp, Addr1, Addr2, Addr3, Addr4, City, St, Zip, Cntry, 
                        AddrType, CustId, AddrHdr, CityLine, FormattedAddr);

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
                AppFtn.Err(CustId, "API Addr Add", x.Message, x.StackTrace);
            }

            j = JsonConvert.SerializeObject(jb);
            h.Content = new StringContent(j, Encoding.UTF8, "application/json");
            return h;
        }

        //Begin database calls

        protected string AddAddrRec(string LblName, string Title, string Comp, string Addr1,
            string Addr2, string Addr3, string Addr4, string City, string St, string Zip,
            string Cntry, string AddrType, string CustId, string AddrHdr, string CityLine,
            string FormattedAddr)
        {
            string r = "";

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.AddrAdd", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustId", CustId);
            cmd.Parameters.AddWithValue("@LblName", LblName);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@Comp", Comp);
            cmd.Parameters.AddWithValue("@Addr1", Addr1);           
            cmd.Parameters.AddWithValue("@Zip", Zip);
            cmd.Parameters.AddWithValue("@Cntry", Cntry);
            cmd.Parameters.AddWithValue("@AddrType", AddrType);
            cmd.Parameters.AddWithValue("@AddrHdr", AddrHdr);
            cmd.Parameters.AddWithValue("@CityLine", CityLine);
            cmd.Parameters.AddWithValue("@FormattedAddr", FormattedAddr);

            if (!String.IsNullOrEmpty(Addr2))
            {
                cmd.Parameters.AddWithValue("@Addr2", Addr2);
            }

            if (!String.IsNullOrEmpty(Addr3))
            {
                cmd.Parameters.AddWithValue("@Addr3", Addr3);
            }

            if (!String.IsNullOrEmpty(Addr4))
            {
                cmd.Parameters.AddWithValue("@Addr4", Addr4);
            }

            if (!String.IsNullOrEmpty(City))
            {
                cmd.Parameters.AddWithValue("@City", City);
            }

            if (!String.IsNullOrEmpty(St))
            {
                cmd.Parameters.AddWithValue("@St", St);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            r = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return r;
        }
    }
}