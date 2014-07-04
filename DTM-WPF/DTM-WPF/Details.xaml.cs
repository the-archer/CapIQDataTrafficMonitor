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
        public Details(int service_id)
        {
            InitializeComponent();

            DateTime end = DateTime.Now;
            DateTime start = DateTime.Now;
            start = start.AddDays(-1);
            UpdateGraph(service_id, "Processed", start, end);


        }

        public void UpdateGraph(int service_id, string metric_name, DateTime start, DateTime end)
        {

            int metric_id = GetMetricID(metric_name);
            MyGlobal.sqlConnection1.Open();
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));

            List<KeyValuePair<DateTime, double>> data = new List<KeyValuePair<DateTime, double>>();
            for (DateTime dt = start; dt <= end; dt = dt.Add(interval))
            {
                
                double per = GetPerformance(service_id, metric_id, dt);
                //Debug.WriteLine(dt);
                data.Add(new KeyValuePair<DateTime, double>(dt, per));

            }
            SqlCommand cmd = new SqlCommand("SELECT service_name FROM BAM_Service_tbl WHERE service_id="+service_id.ToString()+";", MyGlobal.sqlConnection1);
            SqlDataReader rd = cmd.ExecuteReader();
            rd.Read();

            LineSeries t = new LineSeries();
            t.ItemsSource = data;
            t.Title = rd[0].ToString();
            t.DependentValuePath = "Value";
            t.IndependentValuePath = "Key";

            //(lineSeries1.Series[1] as DataPointSeries).ItemsSource = test[key];
            lineSeries1.Series.Add(t);

            rd.Close();

            MyGlobal.sqlConnection1.Close();

                
        }

        public double GetPerformance(int service_id, int metric_id, DateTime dt)
        {
            double per = 0;
            
            SqlCommand cmd = new SqlCommand("BAM_GetPercentage_prc", MyGlobal.sqlConnection1);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = service_id;
            cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric_id;
            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = dt;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                //Debug.WriteLine("Here");
                per = (double)(rd[0]);

            }
            rd.Close();
            return per;
            
        }

       




        public static int GetMetricID(string metric_name)
        {
            int metric_id=0;
            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("BAM_GetMetricID_prc", MyGlobal.sqlConnection1);
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
            MyGlobal.sqlConnection1.Close();
            return metric_id;
        }

      
        
    }
}
