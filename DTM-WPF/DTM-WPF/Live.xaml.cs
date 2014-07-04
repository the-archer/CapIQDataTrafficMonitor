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

        class AutoRefresh
        {
            System.Timers.Timer myTimer = new System.Timers.Timer();

            public void StartTimer(ElapsedEventHandler myEvent, double time)
            {
                myTimer.Elapsed += new ElapsedEventHandler(myEvent);
                myTimer.Interval = time * 1000 * 60;
                myTimer.Enabled = true;
            }
            
          
        }
        public UserControl1()
        {
            InitializeComponent();
           // InitializeComboBox();
            new AutoRefresh();
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

            //updateLiveData(arg1, arg2);
            RefreshGraph();
        }

        public void RefreshGraph()
        {
            RefreshServices();
            //RefreshQueues();
        }

        public void RefreshServices()
        {
            Dictionary<int, string> services = new Dictionary<int, string>();

            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("select * from BAM_service_tbl", MyGlobal.sqlConnection1);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) services.Add(Convert.ToInt32(reader[0]), reader[1].ToString().Replace(" ", "_")); reader.Close();

            cmd = new SqlCommand("BAM_GetMetricId_prc", MyGlobal.sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@metric_name", SqlDbType.VarChar)).Value="Processed";
            reader = cmd.ExecuteReader(); reader.Read(); int metric = Convert.ToInt32(reader[0]); reader.Close();

            foreach (int key in services.Keys)
            {
                var name = services[key]; var per=0D;
                
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var button = FindChild<Button>(Application.Current.MainWindow, name);
                    cmd = new SqlCommand("BAM_GetPercentage_prc", MyGlobal.sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = key;
                    cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric;
                    cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;
                    reader = cmd.ExecuteReader(); reader.Read(); button.Content = reader[0]; per=Convert.ToInt32(reader[0]); reader.Close();

                    cmd = new SqlCommand("BAM_GetDisplayColour_prc", MyGlobal.sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric;
                    cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = key;
                    cmd.Parameters.Add(new SqlParameter("@per", SqlDbType.Int)).Value = per;
                    cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;

                    reader = cmd.ExecuteReader(); reader.Read();
                    button.Background = reader[0].ToString().Equals("Red") ? System.Windows.Media.Brushes.Red : (reader[0].ToString().Equals                    ("Green") ? System.Windows.Media.Brushes.Red : System.Windows.Media.Brushes.Orange);
                    reader.Close();
                }));
            }
            MyGlobal.sqlConnection1.Close();
        }

        public void RefreshQueues()
        {
            for (int s_id = 1; s_id < 6; s_id++)
            {
                Tuple<double, int> pending = GetPending(s_id);
                if(GlobalClass.glob_pending.Count<=s_id)
                    GlobalClass.glob_pending.Add(new Tuple<int, int>(pending.Item2, (int)(((pending.Item2) * 100) / (pending.Item1))));
                else
                GlobalClass.glob_pending[s_id] = new Tuple<int, int>(pending.Item2, (int)(((pending.Item2) * 100) / (pending.Item1)));
                //
                if (pending.Item2 == -1)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        //pb_contentsearch.Foreground=S
                        //pb_contentsearch.Background = System.Windows.Media.Brushes.Blue;
                    }));
                   
                }
                this.Dispatcher.Invoke((Action)(() =>
                {
                    
                    switch (s_id)
                    {
                        case 1:
                            pb_contentsearch.Value = pending.Item1;
                            pb_contentsearch.ToolTip = pending.Item2;
                            break;
                        case 2:
                          pb_workflowloader.Value = pending.Item1;
                          pb_workflowloader.ToolTip = pending.Item2;
                          break;
                        case 3:
                          pb_contentsearchrep.Value = pending.Item1;
                          pb_contentsearchrep.ToolTip = pending.Item2;
                          break;
                        case 4:
                          pb_physicalfilerep.Value = pending.Item1;
                          pb_physicalfilerep.ToolTip = pending.Item2;
                          break;
                        case 5:
                          pb_versioncreation.Value = pending.Item1;
                          pb_versioncreation.ToolTip = pending.Item2;
                          break;


                            


                    }
                    


                    
                }));
               



            }

           

            return;
        }

        public Tuple<double, int> GetPending(int s_id)
        {

            int metric_id = Details.GetMetricID("Pending");
            MyGlobal.sqlConnection1.Open();
            double per = 0;

            SqlCommand cmd = new SqlCommand("BAM_GetPercentage_prc", MyGlobal.sqlConnection1);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = s_id;
            cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric_id;
            cmd.Parameters.Add(new SqlParameter("@date", SqlDbType.DateTime)).Value = DateTime.Now;
            SqlDataReader rd = cmd.ExecuteReader();
            int value = -1;
            while (rd.Read())
            {

                per = (double)(rd[0]);
                value = Convert.ToInt32(rd[1]);

            }
            rd.Close();
            MyGlobal.sqlConnection1.Close();
            return new Tuple<double, int>(per, value);
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


        
        public void updateLiveData(int metric, string metric_name)
        {
            Debug.WriteLine(metric);
            //Debug.WriteLine("Is it here?");

            GlobalClass.data.Clear();

            this.Dispatcher.Invoke((Action)(() =>
            {
                dataGrid1.ItemsSource = null;
                dataGrid1.Items.Refresh();
            }));
           
            DateTime time = DateTime.Now;

            GlobalClass.data = getStats(metric, time); GlobalClass.metric1 = metric; GlobalClass.time1 = time;
            try
            {
                if (GlobalClass.win1.IsEnabled) GlobalClass.win1.Close();
            }
            catch (Exception e)
            {
            }
            
            
            
            
            this.Dispatcher.Invoke((Action)(() =>
            {
                Debug.WriteLine("Start");
                ObservableCollection<Tuple<string, int, int, float, string>> list = new ObservableCollection<Tuple<string, int, int, float, string>>((from item in GlobalClass.data select item.Value));
                dataGrid1.ItemsSource = list;
                dataGrid1.Columns[0].Header = "Service Name";
                dataGrid1.Columns[1].Header = metric_name;
                dataGrid1.Columns[2].Header = "Baseline Value";
                dataGrid1.Columns[3].Header = "Percentage";
                dataGrid1.Columns[4].Header = "Colour";
                dataGrid1.Items.Refresh();
            }));
         
            Debug.WriteLine("Closing");
            
        }

        public void GetDetails(int service_id)
        {

            contentControl1.Content = new Details(service_id);

            return;
        }

        public static Dictionary<int, Tuple<string, int, int, float, string>> getStats(int metric, DateTime time)
        {
            Dictionary<int, Tuple<string, int, int, float, string>> localdata = new Dictionary<int, Tuple<string, int, int, float, string>>();
            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("get_service_tbl", MyGlobal.sqlConnection1);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                localdata.Add(Convert.ToInt32(rd[0]), Tuple.Create(rd[1].ToString(), -1, -1, 0.0F, ""));
            }
            rd.Close();
            List<int> keys = new List<int>(localdata.Keys);
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
                    localdata[item] = Tuple.Create(localdata[item].Item1, Convert.ToInt32(rd[0]), localdata[item].Item3, localdata[item].Item4, localdata[item].Item5);
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
                    per = (float)(localdata[item].Item2) / (Convert.ToInt32(rd[0])) * 100;
                    Debug.WriteLine("{0} {1}", rd[0], rd[1]);
                    localdata[item] = Tuple.Create(localdata[item].Item1, localdata[item].Item2, Convert.ToInt32(rd[0]), per, localdata[item].Item5);
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
                    localdata[item] = Tuple.Create(localdata[item].Item1, localdata[item].Item2, localdata[item].Item3, localdata[item].Item4, rd[0].ToString());
                }
                rd.Close();
            }
            MyGlobal.sqlConnection1.Close();
            return localdata;
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            double time;
            if (Double.TryParse(textBox1.Text, out time))
            {
                if (time > 0)
                {
                    AutoRefresh AR = new AutoRefresh();
                    AR.StartTimer(myEvent, Convert.ToDouble(textBox1.Text));
                }
            }
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           BindingOperations.ClearAllBindings(dataGrid1); 
           GlobalClass.data.Clear();
           AutoRefresh AR = new AutoRefresh();
           AR.StartTimer(myEvent, Convert.ToDouble(textBox1.Text));
        }

        private void service_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender; var name = button.Name.Replace("_"," "); 
            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand("select service_id from bam_service_tbl where service_name = '"+name+"';",                                                   MyGlobal.sqlConnection1);
            SqlDataReader reader = cmd.ExecuteReader(); reader.Read();
            int id = Convert.ToInt32(reader[0]);
            MyGlobal.sqlConnection1.Close();
            GetDetails(id);
        }

        public static T FindChild<T>(DependencyObject parent, string childName)
   where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            GetDetails(Convert.ToInt32(textBox2.Text));

        }

    }

    public class GlobalClass
    {
        public static System.Timers.Timer myTimer = new System.Timers.Timer();
        public static Dictionary<int, Tuple<string, int, int, float, string>> data = new Dictionary<int, Tuple<string, int, int, float, string>>();
        public static ObservableCollection<Tuple<int, string, int, int, float, string>> dataobs = new ObservableCollection<Tuple<int,string,int,int,float,string>>();
        public static int metric1;
        public static DateTime time1;
        public static MainWindow1 win1;
        public static List<Tuple<int, int>> glob_pending = new List<Tuple<int, int>>(){new Tuple<int, int>(0,0)};
        //public static List<int> test = new List<int>(){2, 3, 4, 5, 23, 43, 43, 43};
    }

}
