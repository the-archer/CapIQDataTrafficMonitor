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
using System.Windows.Threading;

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            InitializeComboBox();

            DTimer dtimer = new DTimer();
            

            dtimer.Start(10);
            
            Debug.Write("Hello");
            
            //Debug.Write(dtimer.DoSomething);
            
            
          
        }
        public void Testing()
        {
            Debug.WriteLine("testing");
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            updateLiveData(Convert.ToInt32(comboBox1.SelectedValue), (comboBox1.Text.ToString()));
            
        }

        private void updateLiveData(int metric, string metric_name)
        {
            Debug.Write(metric);
            DateTime time = DateTime.Now;
            Dictionary<int, Tuple<string, int, int, float, string>> data = new Dictionary<int, Tuple<string, int, int, float, string>>();
            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("get_service_tbl", MyGlobal.sqlConnection1);
            cmd.CommandType=CommandType.StoredProcedure;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                data.Add(Convert.ToInt32(rd[0]), Tuple.Create(rd[1].ToString(), -1, -1, 0.0F, ""));
            }
            rd.Close();
            List<int> keys = new List<int>(data.Keys);
            string day = time.DayOfWeek.ToString();
            foreach (var item in keys)
            {
               cmd = new SqlCommand("getvalue_service_metrics_tbl", MyGlobal.sqlConnection1);
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
               cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = item;
               cmd.Parameters.Add(new SqlParameter("@first", SqlDbType.DateTime)).Value = time;

               rd = cmd.ExecuteReader();
               while (rd.Read())
               {
                   Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                   data[item] = Tuple.Create(data[item].Item1, Convert.ToInt32(rd[0]), data[item].Item3, data[item].Item4, data[item].Item5);
               }
               rd.Close();
               cmd = new SqlCommand("getvalue_baseline_tbl", MyGlobal.sqlConnection1);
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
               cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = item;
               cmd.Parameters.Add(new SqlParameter("@first", SqlDbType.DateTime)).Value = time;
               cmd.Parameters.Add(new SqlParameter("@day", SqlDbType.Text)).Value = day;
               rd = cmd.ExecuteReader();
               float per = 0.0F;
               while (rd.Read())
               {
                   per = (float)(data[item].Item2) / (Convert.ToInt32(rd[0])) * 100;
                   Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                   data[item] = Tuple.Create(data[item].Item1, data[item].Item2, Convert.ToInt32(rd[0]), per, data[item].Item5);
               }
               rd.Close();


               cmd = new SqlCommand("getcolour_display_colour_tbl", MyGlobal.sqlConnection1);
               cmd.CommandType = CommandType.StoredProcedure;
               cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
               cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = item;
               cmd.Parameters.Add(new SqlParameter("@per", SqlDbType.Float)).Value = per;
               
               rd = cmd.ExecuteReader();
               
               while (rd.Read())
               {
                   Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                   data[item] = Tuple.Create(data[item].Item1, data[item].Item2, data[item].Item3, data[item].Item4, rd[0].ToString());
               }
               rd.Close();

               dataGrid1.ItemsSource = data.Values;
               dataGrid1.Columns[0].Header = "Service Name";
               dataGrid1.Columns[1].Header = metric_name;
               dataGrid1.Columns[2].Header = "Baseline Value";
               dataGrid1.Columns[3].Header = "Percentage";
               dataGrid1.Columns[4].Header = "Colour";

            }



            MyGlobal.sqlConnection1.Close();
            
            

        }


        public class DTimer
        {
            private DispatcherTimer timer;
            public event Action<int> DoSomething;

            private int _timesCalled = 0;

            public DTimer()
            {
                timer = new DispatcherTimer();
            }
            public void Start(int PeriodInSeconds)
            {
                timer.Interval = TimeSpan.FromSeconds(PeriodInSeconds);
                timer.Tick += timer_Task;
                _timesCalled = 0;
                timer.Start();
            }

            public void Stop()
            {
                timer.Stop();
            }
            private void timer_Task(object sender, EventArgs e)
            {
                _timesCalled++;
                //Debug.WriteLine("testing");
                DoSomething(_timesCalled);

            }
            

        }

         
    }


}
