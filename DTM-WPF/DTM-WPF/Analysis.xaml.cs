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
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Collections.ObjectModel;

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for Analysis.xaml
    /// </summary>
    public partial class Analysis : UserControl
    {
        public ObservableCollection<BoolStringClass> checkedServices { get; set; }
        public ObservableCollection<BoolStringClass> checkedMetrics { get; set; }

        public class BoolStringClass
        {
            public string TheText { get; set; }
            public int TheValue { get; set; }
            public bool Checked { get; set; }
        }

        public Analysis(TabItem analysisTab, TabItem liveTab)
        {
            analysisTab.IsSelected = true; liveTab.IsSelected = false;

            InitializeComponent();
            InitializeDateTimePickers();
            InitializeServicesCheckBox(-1);
            InitializeMetricsCheckBox("None");
        }

        public Analysis(int service_id, string metric_name, TabItem anaylsisTab, TabItem liveTab)
        {
            anaylsisTab.IsSelected = true; liveTab.IsSelected = false;

            InitializeComponent();
            InitializeDateTimePickersLast24();
            InitializeServicesCheckBox(service_id);
            InitializeMetricsCheckBox(metric_name);

            Analyse();
        }

        private void InitializeDateTimePickers()
        {
            startDate.Text = endDate.Text = DateTime.Now.Date.ToString();
        }

        private void InitializeDateTimePickersLast24()
        {
            endDate.Text = DateTime.Now.Date.ToString();
            startDate.Text = (DateTime.Now.AddDays(-1)).Date.ToString();
        }

        private void InitializeServicesCheckBox(int service_id)
        {
            List<Tuple<int, string>> services = GetAllServices();
            checkedServices = new ObservableCollection<BoolStringClass>();

            foreach (var item in services)
            {
                if (item.Item1 == service_id)
                {
                    checkedServices.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1, Checked=true});
                }
                else
                    checkedServices.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1 });
            }
            this.DataContext = this;
        }

        private void InitializeMetricsCheckBox(string metric_name)
        {
            List<Tuple<int, string>> services = GetAllMetrics();
            checkedMetrics = new ObservableCollection<BoolStringClass>();

            foreach (var item in services)
            {
                if (item.Item2 == metric_name)
                {
                    Debug.WriteLine(item.Item2);
                    checkedMetrics.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1, Checked=true });
                }
                else
                    checkedMetrics.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1 });
            }
            this.DataContext = this;
        }

        private void CheckService(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            foreach (var item in checkedServices)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = true;
                    break;
                }
            }
        }

        private void CheckMetric(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            foreach (var item in checkedMetrics)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = true;
                    break;
                }
            }
        }

        private void UnCheckService(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            foreach (var item in checkedServices)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = false;
                    break;
                }
            }
        }

        private void UnCheckMetric(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;

            foreach (var item in checkedMetrics)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = false;
                    break;
                }
            }
        }

        private List<Tuple<int, string>> GetAllServices()
        {
            List<Tuple<int, string>> services = new List<Tuple<int, string>>();

            using (SqlConnection sqlConnection1 = new SqlConnection(Global.connstring))
            {
                SqlCommand cmd = new SqlCommand("BAM_GetAllServices_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConnection1.Open(); SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {                  
                    services.Add(new Tuple<int, string>(Convert.ToInt32(rd[0]), Convert.ToString(rd[1])));
                }
                rd.Close();
            }
            return services;
        }

        private List<Tuple<int, string>> GetAllMetrics()
        {
            List<Tuple<int, string>> metrics = new List<Tuple<int, string>>();

            using (SqlConnection sqlConnection1 = new SqlConnection(Global.connstring))
            {
                SqlCommand cmd = new SqlCommand("BAM_GetAllMetrics_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConnection1.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    metrics.Add(new Tuple<int, string>(Convert.ToInt32(rd[0]), Convert.ToString(rd[1])));
                }
                rd.Close();
            }
            return metrics;
        }

        public void Analyse()
        {
            lineSeries1.Series.Clear();

            Debug.WriteLine(startDate.Text);
            DateTime start = Convert.ToDateTime(startDate.SelectedDate);
            start = start.AddHours(startTime.SelectedHour);
            start = start.AddMinutes(startTime.SelectedMinute);
            start = start.AddSeconds(startTime.SelectedSecond);
            Debug.WriteLine(start.ToString());

            Debug.WriteLine(endDate.Text);
            DateTime end = Convert.ToDateTime(endDate.SelectedDate);
            end = end.AddHours(endTime.SelectedHour);
            end = end.AddMinutes(endTime.SelectedMinute);
            end = end.AddSeconds(endTime.SelectedSecond);

            Debug.WriteLine(end.ToString());
            if (end <= start)
            {
                MessageBoxResult error = MessageBox.Show("Start time should be less than the end time.", "Error");
                return;
            }

            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));
            int count_services = 0;
            int count_metrics = 0;
            foreach (var item in checkedServices)
            {
                if (item.Checked)
                {
                    count_services += 1;
                    foreach (var item2 in checkedMetrics)
                    {
                        if (item2.Checked)
                        {
                            count_metrics += 1;
                            displayHistory
                            (start, end, Convert.ToInt32(item2.TheValue), item2.TheText.ToString(), Convert.ToInt32(item.TheValue), item.TheText.ToString(), interval);
                        }
                    }
                }
            }

            if (count_services == 0)
            {
                MessageBoxResult error = MessageBox.Show("Please select atleast one service.", "Error");
                return;
            }

            if (count_metrics == 0)
            {
                MessageBoxResult error = MessageBox.Show("Please select atleast one metric.", "Error");
                return;
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Analyse();
        }

        private void displayHistory(DateTime start, DateTime end, int metric_id, string metric_name, int service_id, string service_name, TimeSpan interval)
        {
            List<KeyValuePair<DateTime, double>> data = new List<KeyValuePair<DateTime, double>>();
            
            for (DateTime dt = start; dt <= end; dt = dt.Add(interval))
            {
                double per = GetPerformance(service_id, metric_id, dt);
                data.Add(new KeyValuePair<DateTime, double>(dt, per));
            }
           
            LineSeries t = new LineSeries();
            t.ItemsSource = data;
            t.DependentValuePath = "Value";
            t.IndependentValuePath = "Key";
            t.Title = service_name + " - " + metric_name;
            lineSeries1.Series.Add(t);
        }

        public double GetPerformance(int service_id, int metric_id, DateTime dt)
        {
            double per = 0;
            using (SqlConnection sqlconnection1 = new SqlConnection(Global.connstring))
            {
                sqlconnection1.Open();
                SqlCommand cmd = new SqlCommand("BAM_GetPercentage_prc", sqlconnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = service_id;
                cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric_id;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = dt;
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    per = (double)(rd[0]);
                }
                rd.Close();
            }
            return per;
        }

        private void buttonClick(object sender, RoutedEventArgs e)
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
            startDate.Text = start.Date.ToString();
            endDate.Text = end.Date.ToString();
        } 
    }
}
