using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Windows.Controls.DataVisualization.Charting;

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {

        int s_id;
        string m_name;
        public Details(int service_id, string metric_name)
        {
            InitializeComponent();
            s_id = service_id;
            m_name = metric_name;
            DateTime end = DateTime.Now;
            DateTime start = DateTime.Now;
            start = start.AddDays(-1);
            //UpdateGraph(service_id, metric_name, start, end);

            
        }

        //public void UpdateGraph(int service_id, string metric_name, DateTime start, DateTime end)
        //{

        //    int metric_id = GetMetricID(metric_name);
        //    using (SqlConnection sqlconnection1 = new SqlConnection(MyGlobal.connstring))
        //    {
        //        sqlconnection1.Open();
        //        TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));

        //        List<KeyValuePair<DateTime, double>> data = new List<KeyValuePair<DateTime, double>>();
        //        for (DateTime dt = start; dt <= end; dt = dt.Add(interval))
        //        {

        //            double per = GetPerformance(service_id, metric_id, dt);
        //            //Debug.WriteLine(dt);
        //            data.Add(new KeyValuePair<DateTime, double>(dt, per));

        //        }
        //        SqlCommand cmd = new SqlCommand("SELECT service_name FROM BAM_Service_tbl WHERE service_id=" + service_id.ToString() + ";", sqlconnection1);
        //        SqlDataReader rd = cmd.ExecuteReader();
        //        rd.Read();
        //        lineSeries1.Series.Clear();
        //        LineSeries t = new LineSeries();
        //        t.ItemsSource = data;
        //        t.Title = rd[0].ToString();
        //        t.DependentValuePath = "Value";
        //        t.IndependentValuePath = "Key";


        //        lineSeries1.Series.Add(t);

        //        rd.Close();

               

        //    }
        //}

        

        public static int GetMetricID(string metric_name)
        {
            int metric_id=0;
            using (SqlConnection sqlConnection1 = new SqlConnection(Global.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("BAM_GetMetricID_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@metric_name", SqlDbType.VarChar)).Value = metric_name;
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    metric_id = Convert.ToInt32(rd[0]);
                }
                if (metric_id == 0)
                {
                    Debug.WriteLine("Failed to get metric_id");

                }
                rd.Close();
               
             
            }
            return metric_id;
        }

        public void buttonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            DateTime end = DateTime.Now;
            DateTime start = DateTime.Now;
            switch (button.Name)
            {
                case "day":
                    start = start.AddDays(-1);
                    break;
                case "week":
                    start = start.AddDays(-7);
                    break;
                case "month":
                    start = start.AddMonths(-1);
                    break;
                case "year":
                    start = start.AddYears(-1);
                    break;
            }
            //UpdateGraph(s_id, m_name, start, end);
        }   
    }
}
