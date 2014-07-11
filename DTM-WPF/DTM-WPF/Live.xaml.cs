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
                SqlCommand cmd = new SqlCommand("BAM_GetAllServices_prc", sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
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
                        var button = getServiceButton(name);
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
                        button.Background= reader[0].ToString().Equals("Red") ? System.Windows.Media.Brushes.Red : (reader[0].ToString().Equals("Green") ?                                          System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Orange);
                       
                        button.Opacity = 1;
                       
                        reader.Close();
                    }));
                }
            }
        }

        public Button getServiceButton(string name)
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
                            pb_contentsearch.ToolTip = ("Percentage: " + (value).ToString()  + "\nPending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 2:
                            pb_workflowloader.Value = pending.Item1;
                            pb_workflowloader.ToolTip = ("Percentage: " + (value).ToString() + "\nPending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 3:
                            pb_contentsearchrep.Value = pending.Item1;
                            pb_contentsearchrep.ToolTip = ("Percentage: " + (value).ToString() + "\nPending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 4:
                            pb_physicalfilerep.Value = pending.Item1;
                            pb_physicalfilerep.ToolTip = ("Percentage: " + (value).ToString() + "\nPending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                        case 5:
                            pb_versioncreation.Value = pending.Item1;
                            pb_versioncreation.ToolTip = ("Percentage: " + (value).ToString()  + "\nPending: " + pending.Item2.ToString() + "\nBaseline: " + baseline.ToString());
                            break;
                    }
                }));
            }
        }

        public Tuple<double, int> GetPending(int s_id)
        {
            int metric_id = GetMetricID("Pending"), value = -1; double per=0;

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

        public int GetMetricID(string metric_name)
        {
            int metric_id = 0;
            using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("BAM_GetMetricID_prc", sqlConnection1);
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


            }
            return metric_id;
        }

        public void GetDetails(int service_id)
        {
            try { MyGlobal.myTimer.Dispose(); }
            catch (Exception exp) { } 
          
            contentControl1.Content = new Analysis(service_id, "Processed");

          
        }

        

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            double time;
            if (Double.TryParse(textBox1.Text, out time))
            {
                if (time > 0)
                {
                    try { MyGlobal.myTimer.Dispose(); }
                    catch (Exception exp) { } 
                    MyGlobal.myTimer = new System.Timers.Timer();
                    MyGlobal.AR.StartTimer(myEvent, Convert.ToDouble(textBox1.Text));
                }
            }
        }

        private void service_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender; var name = button.Name.Replace("_"," ");
            using(SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
            {
                sqlConnection1.Open();
                SqlCommand cmd = new SqlCommand("BAM_GetService_prc", sqlConnection1); cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@serviceName", SqlDbType.VarChar)).Value = name;
                SqlDataReader reader = cmd.ExecuteReader(); reader.Read();
                int id = Convert.ToInt32(reader[0]); reader.Close();
                GetDetails(id);
            }
        }
    }

    public class AutoRefresh
    {
        public void StartTimer(ElapsedEventHandler myEvent, double time)
        {
            MyGlobal.myTimer.Elapsed += new ElapsedEventHandler(myEvent);
            MyGlobal.myTimer.Interval = time * 1000 * 60;
            MyGlobal.myTimer.Enabled = true;
        }
        public void ChangeTime(double time)
        {
            MyGlobal.myTimer.Interval = time * 1000 * 60;
            MyGlobal.myTimer.Enabled = true;
        }
        public void StopTimer()
        {
            MyGlobal.myTimer.Enabled = false;
            MyGlobal.myTimer.Close();
        }
    }
}
