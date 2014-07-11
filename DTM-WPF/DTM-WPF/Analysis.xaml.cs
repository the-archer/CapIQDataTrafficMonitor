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

        public ObservableCollection<BoolStringClass> TheList { get; set; }

        public ObservableCollection<BoolStringClass> TheList2 { get; set; }

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


        public Analysis(int service_id, string metric_name, TabItem ti, TabItem ti2)
        {
            ti.IsSelected = true; ti2.IsSelected = false;
            InitializeComponent();
            
            
            InitializeDateTimePickersLast24();
            
            InitializeServicesCheckBox(service_id);
            InitializeMetricsCheckBox(metric_name);
            Analyse();

        }

        private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            //ZoneText.Text = "Selected Zone Name= " + chkZone.Content.ToString();
            //ZoneValue.Text = "Selected Zone Value= " + chkZone.Tag.ToString();
            foreach (var item in TheList)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = true;
                    break;
                }
                
            }
            //Debug.WriteLine(TheList[Convert.ToInt32(chkZone.Tag)]);
        }

        private void CheckBoxZone_Checked2(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            //ZoneText.Text = "Selected Zone Name= " + chkZone.Content.ToString();
            //ZoneValue.Text = "Selected Zone Value= " + chkZone.Tag.ToString();
            foreach (var item in TheList2)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = true;
                    break;
                }

            }
            //Debug.WriteLine(TheList[Convert.ToInt32(chkZone.Tag)]);
        }


        private void CheckBoxZone_UnChecked(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            //ZoneText.Text = "Selected Zone Name= " + chkZone.Content.ToString();
            //ZoneValue.Text = "Selected Zone Value= " + chkZone.Tag.ToString();
            foreach (var item in TheList)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = false;
                    break;
                }

            }
            //Debug.WriteLine(TheList[Convert.ToInt32(chkZone.Tag)]);
        }


        private void CheckBoxZone_UnChecked2(object sender, RoutedEventArgs e)
        {
            CheckBox chkZone = (CheckBox)sender;
            //ZoneText.Text = "Selected Zone Name= " + chkZone.Content.ToString();
            //ZoneValue.Text = "Selected Zone Value= " + chkZone.Tag.ToString();
            foreach (var item in TheList2)
            {
                if (item.TheValue == Convert.ToInt32(chkZone.Tag))
                {
                    item.Checked = false;
                    break;
                }

            }
            //Debug.WriteLine(TheList[Convert.ToInt32(chkZone.Tag)]);
        } 

        private void InitializeServicesCheckBox(int service_id)
        {
            List<Tuple<int, string>> services = GetAllServices();

            TheList = new ObservableCollection<BoolStringClass>();
            foreach (var item in services)
            {
                if (item.Item1 == service_id)
                {
                    TheList.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1, Checked=true});
                }
                else
                TheList.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1 });

            }
           
            this.DataContext = this;

        }


        private void InitializeMetricsCheckBox(string metric_name)
        {
            List<Tuple<int, string>> services = GetAllMetrics();

            TheList2 = new ObservableCollection<BoolStringClass>();
            foreach (var item in services)
            {
                if (item.Item2 == metric_name)
                {
                    Debug.WriteLine(item.Item2);
                    TheList2.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1, Checked=true });
                }
                else
                TheList2.Add(new BoolStringClass { TheText = item.Item2, TheValue = item.Item1 });
            }
           
            
            this.DataContext = this;

        }

        private List<Tuple<int, string>> GetAllServices()
        {
            List<Tuple<int, string>> services = new List<Tuple<int, string>>();

            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {

                SqlCommand cmd = new SqlCommand("BAM_GetAllServices_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConnection1.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                   
                    services.Add(new Tuple<int, string>(Convert.ToInt32(rd[0]), Convert.ToString(rd[1])));

                }
            }

            return services;
        }



        private List<Tuple<int, string>> GetAllMetrics()
        {
            List<Tuple<int, string>> metrics = new List<Tuple<int, string>>();

            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {

                SqlCommand cmd = new SqlCommand("BAM_GetAllMetrics_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                sqlConnection1.Open();
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {

                    metrics.Add(new Tuple<int, string>(Convert.ToInt32(rd[0]), Convert.ToString(rd[1])));

                }
            }

            return metrics;
        }

      

        private void InitializeDateTimePickers()
        {
            datePicker1.Text = DateTime.Now.Date.ToString();
            datePicker2.Text = DateTime.Now.Date.ToString();
        }

        private void InitializeDateTimePickersLast24()
        {

            datePicker2.Text = DateTime.Now.Date.ToString();
            
            datePicker1.Text = (DateTime.Now.AddDays(-1)).Date.ToString();
        }

        public void Analyse()
        {
            lineSeries1.Series.Clear();

            Debug.WriteLine(datePicker1.Text);
            DateTime start = Convert.ToDateTime(datePicker1.SelectedDate);
            start = start.AddHours(timePicker1.SelectedHour);
            start = start.AddMinutes(timePicker1.SelectedMinute);
            start = start.AddSeconds(timePicker1.SelectedSecond);
            Debug.WriteLine(start.ToString());



            Debug.WriteLine(datePicker2.Text);
            DateTime end = Convert.ToDateTime(datePicker2.SelectedDate);
            end = end.AddHours(timePicker2.SelectedHour);
            end = end.AddMinutes(timePicker2.SelectedMinute);
            end = end.AddSeconds(timePicker2.SelectedSecond);



            Debug.WriteLine(end.ToString());


            if (end <= start)
            {
                MessageBoxResult error = MessageBox.Show("Start time should be less than the end time.", "Error");
                return;
            }

            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));
            int count_services = 0;
            int count_metrics = 0;
            foreach (var item in TheList)
            {
                if (item.Checked)
                {
                    count_services += 1;
                    foreach (var item2 in TheList2)
                    {
                        if (item2.Checked)
                        {
                            count_metrics += 1;
                            displayHistory(start, end, Convert.ToInt32(item2.TheValue), item2.TheText.ToString(), Convert.ToInt32(item.TheValue), item.TheText.ToString(), interval);
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
            //lineSeries1.Series.Clear();


            List<KeyValuePair<DateTime, double>> data = new List<KeyValuePair<DateTime, double>>();
            //Dictionary<int, string> service_names = new Dictionary<int, string>();
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
            using (SqlConnection sqlconnection1 = new SqlConnection(MyGlobal.connstring))
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
                    //Debug.WriteLine("Here");
                    per = (double)(rd[0]);

                }
                rd.Close();

            }

            return per;


        }



        private object var(DateTime dt, float p)
        {
            throw new NotImplementedException();
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

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

            datePicker1.Text = start.Date.ToString();
            datePicker2.Text = end.Date.ToString();
        }

        private void comboBox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

       
    }
}
