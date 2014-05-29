using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Drawing;

namespace DataTrafficMonitor
{
    public class MyGlobal
    {
        public static string connstring = @"Data Source=CIQGUR-ATD133\sqlexpress;Initial Catalog=dtm;Integrated Security=True;Pooling=False";
    }


    [Serializable]
    public class fields
    {
        public string servicename { get; set; }
        public int value { get; set; }
        public string colour { get; set; }
        

    }

    public partial class _Default : System.Web.UI.Page
    {
       

        protected void Button1_Click(object sender, EventArgs e)
        {

        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Button1_Click1(object sender, EventArgs e)
        {
            DateTime time = DateTime.Now;
            string metric = (DropDownList2.Text);

            int year = Convert.ToInt32(TextBox1.Text);
            int month = Convert.ToInt32(TextBox2.Text);
            int day = Convert.ToInt32(TextBox3.Text);
            int hour = Convert.ToInt32(TextBox4.Text);
            int min = Convert.ToInt32(TextBox5.Text);
            time = new DateTime(year, month, day); time = time.AddHours(hour); time = time.AddMinutes(min);

            getStats(time, metric);

        }
        void getStats(DateTime time, string metric)
        {
            //comment
            Debug.Write(DateTime.Now.ToString());

            int m_id = getMetricId(metric);

            Dictionary<int, Tuple<string, int, int, float, string>> s_id = getServiceId();
           
            List<int> keys = new List<int>(s_id.Keys);

            foreach(var item in keys)
            {


                System.Data.SqlClient.SqlConnection sqlConnection1 =
                new System.Data.SqlClient.SqlConnection(MyGlobal.connstring);

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "declare @first datetime set @first='" + time + "' select top(1) value,timestamp from service_metrics_tbl where metric_id="+m_id+" and service_id="+item+" order by ABS(DATEDIFF(day,convert(date,timestamp),convert(date,@first))) ASC, ABS((cast(@first as float)-floor(cast(@first as float)))-(cast(timestamp as float)-floor(cast(timestamp as float)))) ASC;" ;
                sqlConnection1.Open(); cmd.Connection = sqlConnection1;
                SqlDataReader rd = cmd.ExecuteReader();
                Debug.Write(m_id  + " " + item);
                string day = time.DayOfWeek.ToString();
                while (rd.Read())
                {
                    Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                    s_id[item] = Tuple.Create(s_id[item].Item1, Convert.ToInt32(rd[0]), s_id[item].Item3, s_id[item].Item4, s_id[item].Item5);
                }

                cmd.CommandText = "declare @first datetime set @first='" + time + "' select top(1) value,start_time, end_time from baseline_tbl where metric_id=" + m_id + " and service_id=" + item + " and day='" + day + "' and DateDiff(second, Cast(start_time as time), Cast(@first as time))>=0 order by  ABS(DateDiff(second, Cast(start_time as time), Cast(@first as time))) ASC;";
                rd.Close();
                rd = cmd.ExecuteReader();
                float per=0.0F;
                while (rd.Read())
                {
                    per = (float)(s_id[item].Item2) / (Convert.ToInt32(rd[0])) * 100;
                    Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                    s_id[item] = Tuple.Create(s_id[item].Item1, s_id[item].Item2, Convert.ToInt32(rd[0]), per, s_id[item].Item5);
                }
                rd.Close();
                cmd.CommandText = "select top(1) colour, min_threshold from display_colour_tbl where service_id = " + item + " and metric_id = " + m_id + " and min_threshold <= " + per + " order by min_threshold DESC;";
                rd = cmd.ExecuteReader();
                
                while (rd.Read())
                {
                    Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                    s_id[item] = Tuple.Create(s_id[item].Item1, s_id[item].Item2, s_id[item].Item3, s_id[item].Item4, rd[0].ToString());
                }
                
                sqlConnection1.Close();


            }
            GridView1.DataSource = s_id.Values;
            

            GridView1.DataBind();

            foreach (GridViewRow row in GridView1.Rows)
            {
                row.ForeColor = ColorTranslator.FromHtml(row.Cells[4].Text);
            }
               
            
            GridView1.HeaderRow.Cells[0].Text = "Service Name";
            GridView1.HeaderRow.Cells[1].Text = metric;
            GridView1.HeaderRow.Cells[2].Text = "Baseline Value";
            GridView1.HeaderRow.Cells[3].Text = "Percentage";
            GridView1.HeaderRow.Cells[4].Text = "Colour";

        }

        private Dictionary<int, Tuple<string, int, int, float, string>> getServiceId()
        {
            Dictionary<int, Tuple<string, int, int, float, string>>  service= new Dictionary<int,Tuple<string,int,int,float,string>>();
            System.Data.SqlClient.SqlConnection sqlConnection1 = new System.Data.SqlClient.SqlConnection(MyGlobal.connstring);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select service_id,service_name from service_tbl;";
            sqlConnection1.Open(); cmd.Connection = sqlConnection1;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {

                service.Add(Convert.ToInt32(rd[0]), Tuple.Create(rd[1].ToString(), -1, -1, 0.0F, ""));
            }
            sqlConnection1.Close();
            return service;
        }

        int getMetricId(string metric)
        {

            System.Data.SqlClient.SqlConnection sqlConnection1 =
            new System.Data.SqlClient.SqlConnection(MyGlobal.connstring);

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select metric_id from metrics_tbl where metric_name like '"+metric+"';";
            sqlConnection1.Open(); cmd.Connection = sqlConnection1;
            SqlDataReader rd = cmd.ExecuteReader(); rd.Read();
            return Convert.ToInt32(rd[0]);
                            
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void DTM_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }

        protected void DropDownList2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            DateTime timenow = DateTime.Now;

            int year = timenow.Year;
            int month = timenow.Month;
            int date = timenow.Day;
            int hour = timenow.Hour;
            int min = timenow.Minute;
            TextBox1.Text = year.ToString();
            TextBox2.Text = month.ToString();
            TextBox3.Text = date.ToString();
            TextBox4.Text = hour.ToString();
            TextBox5.Text = min.ToString();

        }


    }


  

}
