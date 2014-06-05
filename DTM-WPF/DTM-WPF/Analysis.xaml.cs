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
            SqlDataAdapter da = new SqlDataAdapter("Select metric_name, metric_id from metrics_tbl", MyGlobal.sqlConnection1);
            DataSet ds = new DataSet();

            da.Fill(ds, "Metric");
            comboBox1.ItemsSource = ds.Tables[0].DefaultView;
            comboBox1.DisplayMemberPath = ds.Tables[0].Columns["metric_name"].ToString();
            comboBox1.SelectedValuePath = ds.Tables[0].Columns["metric_id"].ToString();
            comboBox1.SelectedIndex = 0;


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
            DateTime interval = new DateTime();
            interval = interval.AddMinutes(30);
            displayHistory(start, end, Convert.ToInt32(comboBox1.SelectedValue), comboBox1.Text.ToString(), interval);

        }

        private void displayHistory(DateTime start, DateTime end, int metric, string metric_name, DateTime interval)
        {
            List<KeyValuePair<int, int>> test = new List<KeyValuePair<int, int>>();
           
            test.Add(new KeyValuePair<int,int>(2, 3));
            test.Add(new KeyValuePair<int,int>(4, 1));
            test.Add(new KeyValuePair<int,int>(5, 2));
             test.Add(new KeyValuePair<int,int>(6, 7));
            (lineSeries1.Series[0] as DataPointSeries).ItemsSource = test;
           
            Debug.Write("here");
            
        }
    }
}
