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

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for Analysis.xaml
    /// </summary>
    public partial class Analysis : UserControl
    {
        public Analysis()
        {
            InitializeComponent();
            InitializeDateTimePickers();
            InitializeComboBox();
            
        }

        private void InitializeComboBox()
        {
            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT metric_name, metric_id from BAM_Metric_tbl", sqlConnection1);
                DataSet ds = new DataSet();

                da.Fill(ds, "Metric");
                comboBox1.ItemsSource = ds.Tables[0].DefaultView;
                comboBox1.DisplayMemberPath = ds.Tables[0].Columns["metric_name"].ToString();
                comboBox1.SelectedValuePath = ds.Tables[0].Columns["metric_id"].ToString();
                comboBox1.SelectedIndex = 0;
            }

        }

        private void InitializeDateTimePickers()
        {
            datePicker1.Text = DateTime.Now.Date.ToString();
            datePicker2.Text = DateTime.Now.Date.ToString();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(datePicker1.Text);
            DateTime start = Convert.ToDateTime(datePicker1.SelectedDate);
            start=start.AddHours(timePicker1.SelectedHour);
            start=start.AddMinutes(timePicker1.SelectedMinute);
            start=start.AddSeconds(timePicker1.SelectedSecond);
            Debug.WriteLine(start.ToString());
           
            
            
            Debug.WriteLine(datePicker2.Text);
            DateTime end = Convert.ToDateTime(datePicker2.SelectedDate);
            end = end.AddHours(timePicker2.SelectedHour);
            end = end.AddMinutes(timePicker2.SelectedMinute);
            end = end.AddSeconds(timePicker2.SelectedSecond);



            Debug.WriteLine(end.ToString());

            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20) );
            
            
            displayHistory(start, end, Convert.ToInt32(comboBox1.SelectedValue), comboBox1.Text.ToString(), interval);

        }

        private void displayHistory(DateTime start, DateTime end, int metric, string metric_name, TimeSpan interval)
        {
            lineSeries1.Series.Clear();

            Dictionary<int, List<KeyValuePair<DateTime, double>>> test = new Dictionary<int, List<KeyValuePair<DateTime, double>>>();

            Dictionary<int, string> service_names = new Dictionary<int, string>();
            for (DateTime dt = start; dt <= end; dt = dt.Add(interval))
            {
                Dictionary<int, Tuple<string, int, int, double, string>> data = UserControl1.getStats(metric, dt);
                foreach (var item in data)
                {
                    if(test.ContainsKey(item.Key))
                    {
                        test[item.Key].Add(new KeyValuePair<DateTime, double>(dt, item.Value.Item4));
                    }
                    else
                    {

                        test.Add(item.Key, new List<KeyValuePair<DateTime, double>>());
                        test[item.Key].Add(new KeyValuePair<DateTime, double>(dt, item.Value.Item4));
                        service_names.Add(item.Key, item.Value.Item1);

                    }
                   


                }
                
                

            }
            //  test.Add(new KeyValuePair<int,int>(2, 3));
            //test.Add(new KeyValuePair<int,int>(4, 1));
            //test.Add(new KeyValuePair<int,int>(5, 2));
            // test.Add(new KeyValuePair<int,int>(6, 7));

            var set = test.Keys; int i = 0;
            foreach(var key in set)
            {
                LineSeries t = new LineSeries();
                t.ItemsSource=test[key];
                t.Title = service_names[key];
                t.DependentValuePath = "Value";
                t.IndependentValuePath = "Key";
                //(lineSeries1.Series[1] as DataPointSeries).ItemsSource = test[key];
                lineSeries1.Series.Add(t);
            }
            
            //lineSeries1.Series[1] as DataPointSeries).ItemsSource = test;
            Debug.WriteLine(test);
            Debug.Write("here");
            
        }

        private object var(DateTime dt, float p)
        {
            throw new NotImplementedException();
        }

        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
