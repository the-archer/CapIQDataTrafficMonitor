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
using System.Threading;
using System.Collections.ObjectModel;


namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    /// 

    public partial class UserControl1 : UserControl
    {

        

        public UserControl1()
        {
            InitializeComponent();
            RefreshGraph();
            GlobalClass.AR.StartTimer(myEvent, 0.1);
            
        }
       
        public void myEvent(object source, ElapsedEventArgs e)
        {
            Debug.WriteLine("Timer working");
            RefreshGraph();
        }

        public void RefreshGraph()
        {
            Debug.WriteLine(DateTime.Now);
            RefreshQueues();
            RefreshServices();
        }

        public void RefreshServices()
        {
            Dictionary<int, string> services = new Dictionary<int, string>();

            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("select * from BAM_service_tbl", sqlConnection1);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) services.Add(Convert.ToInt32(reader[0]), reader[1].ToString().Replace(" ", "_")); reader.Close();

                cmd = new SqlCommand("BAM_GetMetricId_prc", sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@metric_name", SqlDbType.VarChar)).Value = "Processed";
                reader = cmd.ExecuteReader(); reader.Read(); int metric = Convert.ToInt32(reader[0]); reader.Close();

                foreach (int key in services.Keys)
                {
                    var name = services[key]; var per = 0D;

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        var button = getService(name);
                        cmd = new SqlCommand("BAM_GetPercentage_prc", sqlConnection1); 
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = key;
                        cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric;
                        cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;
                        reader = cmd.ExecuteReader(); reader.Read(); button.Content = reader[0]; per = Convert.ToInt32(reader[0]);
                        var baseline = Convert.ToDouble(reader[1]) * 100.0 / per;
                        button.ToolTip = "Processed : " + reader[1] + "\nBaseline : " + (int)baseline; reader.Close();

                        cmd = new SqlCommand("BAM_GetDisplayColour_prc", sqlConnection1);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
                        cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = key;
                        cmd.Parameters.Add(new SqlParameter("@per", SqlDbType.Int)).Value = per;
                        cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;

                        reader = cmd.ExecuteReader(); reader.Read();
                        button.Background = reader[0].ToString().Equals("Red") ? System.Windows.Media.Brushes.Red : (reader[0].ToString().Equals("Green") ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Orange);
                        reader.Close();
                    }));
                }
            }
        }

        public Button getService(string name)
        {
            switch (name)
            {
                case "Version_Creation":
                    return Version_Creation;
                case "Content_Search":
                    return Content_Search;
                case "CS_Replication":
                    return CS_Replication;
                case "Physical_File_Replication":
                    return Physical_File_Replication;
                case "Workflow_Loader":
                    return Workflow_Loader;
            }
            return null;
        }

        public void RefreshQueues()
        {
            for (int s_id = 1; s_id < 6; s_id++)
            {
                Tuple<double, int> pending = GetPending(s_id);
                if (pending.Item1 == -1) continue;
                int baseline = (int)(((pending.Item2) * 100) / (pending.Item1));
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var value = (double)pending.Item1;

                    switch (s_id)
                    {
                        case 1:
                            pb_contentsearch.Value = value;
                            pb_contentsearch.ToolTip = ("Pending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 2:
                            pb_workflowloader.Value = pending.Item1;
                            pb_workflowloader.ToolTip = ("Pending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 3:
                            pb_contentsearchrep.Value = pending.Item1;
                            pb_contentsearchrep.ToolTip = ("Pending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 4:
                            pb_physicalfilerep.Value = pending.Item1;
                            pb_physicalfilerep.ToolTip = ("Pending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 5:
                            pb_versioncreation.Value = pending.Item1;
                            pb_versioncreation.ToolTip = ("Pending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                    }
                }));
            }
        }

        public Tuple<double, int> GetPending(int s_id)
        {

            int metric_id = Details.GetMetricID("Pending"), value = 10; double per=0;

            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                per = 0;

                SqlCommand cmd = new SqlCommand("BAM_GetPercentage_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = s_id;
                cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric_id;
                cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;
                SqlDataReader rd = cmd.ExecuteReader();
                value = -1;
                while (rd.Read())
                {

                    per = (double)(rd[0]);
                    value = Convert.ToInt32(rd[1]);
                }
                rd.Close();
            }
            return new Tuple<double, int>(per, value);
        }
        public void Testing()
        {
            Debug.WriteLine("testing");


            
        }

       


        
        //public void updateLiveData(int metric, string metric_name)
        //{
        //    Debug.WriteLine(metric);
        //    //Debug.WriteLine("Is it here?");

        //    GlobalClass.data.Clear();

        //    this.Dispatcher.Invoke((Action)(() =>
        //    {
        //        dataGrid1.ItemsSource = null;
        //        dataGrid1.Items.Refresh();
        //    }));
           
        //    DateTime time = DateTime.Now;

        //    GlobalClass.data = getStats(metric, time); GlobalClass.metric1 = metric; GlobalClass.time1 = time;
        //    try
        //    {
        //        if (GlobalClass.win1.IsEnabled) GlobalClass.win1.Close();
        //    }
        //    catch (Exception e)
        //    {
        //    }
            
            
            
            
        //    this.Dispatcher.Invoke((Action)(() =>
        //    {
        //        Debug.WriteLine("Start");
        //        ObservableCollection<Tuple<string, int, int, float, string>> list = new ObservableCollection<Tuple<string, int, int, float, string>>((from item in GlobalClass.data select item.Value));
        //        dataGrid1.ItemsSource = list;
        //        dataGrid1.Columns[0].Header = "Service Name";
        //        dataGrid1.Columns[1].Header = metric_name;
        //        dataGrid1.Columns[2].Header = "Baseline Value";
        //        dataGrid1.Columns[3].Header = "Percentage";
        //        dataGrid1.Columns[4].Header = "Colour";
        //        dataGrid1.Items.Refresh();
        //    }));
         
        //    Debug.WriteLine("Closing");
            
        //}

        public void GetDetails(int service_id)
        {
            GlobalClass.AR.StopTimer();           
            contentControl1.Content = new Details(service_id, "Processed");
        }

        public static Dictionary<int, Tuple<string, int, int, double, string>> getStats(int metric, DateTime time)
        {
            Dictionary<int, Tuple<string, int, int, double, string>> localdata = 
            new Dictionary<int, Tuple<string, int, int, double, string>>();
            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("BAM_GetAllServices_prc", sqlConnection1);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    localdata.Add(Convert.ToInt32(rd[0]), Tuple.Create(rd[1].ToString(), -1, -1, 0.0, ""));
                }
                rd.Close();
                List<int> keys = new List<int>(localdata.Keys);
                string day = time.DayOfWeek.ToString();
                foreach (var item in keys)
                {
                    cmd = new SqlCommand("BAM_GetPercentage_prc", sqlConnection1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric;
                    cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = item;
                    cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = time;
                    double per = 0;
                    rd = cmd.ExecuteReader();
                    while (rd.Read())
                    {
                        Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                        localdata[item] = Tuple.Create(localdata[item].Item1, Convert.ToInt32(rd[1]), localdata[item].Item3,                                        Convert.ToDouble(rd[0]), localdata[item].Item5);
                        per = Convert.ToDouble(rd[0]);
                    }
                    rd.Close();
                    cmd = new SqlCommand("BAM_GetDisplayColour_prc", sqlConnection1);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
                    cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = item;
                    cmd.Parameters.Add(new SqlParameter("@per", SqlDbType.Float)).Value = per;
                    cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;
                    rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                        localdata[item] = Tuple.Create(localdata[item].Item1, localdata[item].Item2, localdata[item].Item3, localdata[item].Item4, rd[0].ToString());
                    }
                    rd.Close();
                }
            }
            return localdata;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            double time;
            if (Double.TryParse(textBox1.Text, out time))
            {
                if (time > 0)
                {
                    GlobalClass.AR.ChangeTime(Convert.ToDouble(textBox1.Text));
                }
            }
        }

        private void service_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender; var name = button.Name.Replace("_"," ");
            using(SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("select service_id from bam_service_tbl where service_name = '"+name+"';",                                                   sqlConnection1);
                SqlDataReader reader = cmd.ExecuteReader(); reader.Read();
                int id = Convert.ToInt32(reader[0]); reader.Close();
                GetDetails(id);
            }
        }
    }


    public class AutoRefresh
    {
        //System.Timers.Timer myTimer = new System.Timers.Timer();

        public void StartTimer(ElapsedEventHandler myEvent, double time)
        {
            GlobalClass.myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            GlobalClass.myTimer.Interval = time * 1000 * 60;
            GlobalClass.myTimer.Enabled = true;
        }
        public void ChangeTime(double time)
        {
            GlobalClass.myTimer.Interval = time * 1000 * 60;
            GlobalClass.myTimer.Enabled = true;
        }
        public void StopTimer()
        {
            GlobalClass.myTimer.Enabled = false;
            
        }


    }

    public class GlobalClass
    {
        public static AutoRefresh AR = new AutoRefresh();
        public static System.Timers.Timer myTimer = new System.Timers.Timer();
        public static Dictionary<int, Tuple<string, int, int, float, string>> data = new Dictionary<int, Tuple<string, int, int, float, string>>();
        public static ObservableCollection<Tuple<int, string, int, int, float, string>> dataobs = new ObservableCollection<Tuple<int,string,int,int,float,string>>();
        public static int metric1;
        public static DateTime time1;
        //public static MainWindow1 win1;
        public static List<Tuple<int, int>> glob_pending = new List<Tuple<int, int>>(){new Tuple<int, int>(0,0)};
        //public static List<int> test = new List<int>(){2, 3, 4, 5, 23, 43, 43, 43};
        public static int first = 1;
        
    }

}
