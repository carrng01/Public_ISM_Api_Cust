using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Cust.Controllers
{
    public class AppFtn
    {
        public static void Err(string CustId, string Src, string Msg, string Stk)
        {
            string url = "http://api.corp.ism.ws/err/post";

            using (var client = new HttpClient())
            {
                var p = new Dictionary<string, string>();
                p.Add("CustId", CustId);
                p.Add("Src", Src);
                p.Add("Msg", Msg);
                p.Add("Stk", Stk);

                string j = JsonConvert.SerializeObject(p);

                var response = client.PostAsync(url, new StringContent(j, Encoding.UTF8, "application/json")).Result;
            }
        }

        public static DataTable CustByEmailRec(string Email)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGetByEmail", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", Email);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }

        public static DataTable CustByAddrRec(string Cntry, string Zip, string FirstName, string LastName)
        {
            DataTable Tbl = new DataTable();

            string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["Api"].ConnectionString;
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand("dbo.CustGetByAddr", conn);
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Cntry", Cntry);
            cmd.Parameters.AddWithValue("@Zip", Zip);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@LastName", LastName);

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            conn.Open();
            da.Fill(Tbl);
            conn.Close();

            return Tbl;
        }
    }
}