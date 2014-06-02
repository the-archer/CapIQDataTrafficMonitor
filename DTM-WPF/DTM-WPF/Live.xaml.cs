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
using System.Timers;

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 

    public class GlobalClass
    {
            
    }


    public partial class UserControl1 : UserControl
    {


        class AutoRefresh
        {

              System.Timers.Timer myTimer = new System.Timers.Timer();

              public void StartTimer(ElapsedEventHandler myEvent, double time)
              {

                  myTimer.Elapsed += new ElapsedEventHandler(myEvent);
                  myTimer.Interval = time*1000;
                  myTimer.Enabled = true;
              }

        }
        public UserControl1()
        {
            InitializeComponent();
            InitializeComboBox();
             AutoRefresh AR = new AutoRefresh();
            //DTimer dtimer = new DTimer();
            
            //DelayedExecutionService.DelayedExecute(() => Debug.Write("Hello"));
            //dtimer.Start(10);

            //AutoRefresh AR = new AutoRefresh();
            //AR.StartTimer(myEvent, Convert.ToDouble(textBox1.Text));
          
            
            //Debug.Write(dtimer.DoSomething);
            
            
          
        }

        public void myEvent(object source, ElapsedEventArgs e)
        {

            Debug.WriteLine("Timer working");
            int arg1=0;
            string arg2="10";
            this.Dispatcher.Invoke((Action)(() =>
            {

                arg1 = Convert.ToInt32(comboBox1.SelectedValue);
                arg2 = (comboBox1.Text.ToString());
            }));
            updateLiveData(arg1, arg2);
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


           
           

            AR.StartTimer(myEvent, Convert.ToDouble(textBox1.Text));
            
        }

        public void updateLiveData(int metric, string metric_name)
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


               this.Dispatcher.Invoke((Action)(() =>
               {


                   dataGrid1.ItemsSource = data.Values;
                   dataGrid1.Columns[0].Header = "Service Name";
                   dataGrid1.Columns[1].Header = metric_name;
                   dataGrid1.Columns[2].Header = "Baseline Value";
                   dataGrid1.Columns[3].Header = "Percentage";
                   dataGrid1.Columns[4].Header = "Colour";
               }));


            }




            Debug.WriteLine("CLosing");
            MyGlobal.sqlConnection1.Close();
            
            

        }


        

        

         
    }


}
