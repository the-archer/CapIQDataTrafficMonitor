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

            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));
            int metric_id = GetMetricID(metric_name);
            List<KeyValuePair<DateTime, float>> data = new List<KeyValuePair<DateTime, float>>();
            for (DateTime dt = start; dt <= end; dt = dt.Add(interval))
            {
                float per = GetPerformance(service_id, metric_id, dt);


            }


            

                
        }

        public float GetPerformance(int service_id, int metric_id, DateTime dt)
        {
            float per = 0;

            return per;
            
        }

        public int GetMetricID(string metric_name)
        {
            int metric_id=0;
            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("BAM_GetMetricID_prc", MyGlobal.sqlConnection1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                metric_id = Convert.ToInt32(rd[0]);
            }
            if (metric_id == 0)
            {
                Debug.WriteLine("Failed to get metric_id");
                
            }

            return metric_id;
        }

        
    }
}
