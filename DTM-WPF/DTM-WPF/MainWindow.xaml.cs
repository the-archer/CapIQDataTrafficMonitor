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

namespace DTM_WPF
{

   
       
     /// <summary>  
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void getLive(object sender, RoutedEventArgs e)
        {
            contentControl1.Content = new UserControl1(liveTab,analysisTab);
        }

        private void getAnalysis(object sender, RoutedEventArgs e)
        {
            contentControl1.Content = new Analysis(analysisTab,liveTab);
        }
    }

    public class Global
    {
        public static string connstring = @"Data Source=figo\ford;Initial Catalog=Dashboard;Integrated Security=True";
        public static AutoRefresh AR = new AutoRefresh();
        public static System.Timers.Timer myTimer = new System.Timers.Timer();
    }

   

    //    private void caller()
    //    {
    //        Debug.Write("Hello sd!");





    //        MyGlobal.sqlConnection1.Open();
    //        //cmd.CommandType = System.Data.CommandType.Text;
    //        //cmd.Parameters.AddWithValue("@id1", 10);
    //        //cmd.Parameters.AddWithValue("@name1", "hello");
    //        //cmd.CommandText = "insert into baseline_tbl values ('Monday','0:0:00.0000000','0:00:0.0000000',1,1,1000);";
    //        //cmd.CommandText = "select count(*) from display_colour_tbl";
    //        //cmd.CommandText = "select * from service_metrics_tbl; ";
           
    //        Debug.WriteLine("Success");
            
    //        Debug.WriteLine("Success");
    //        //Console.Write(cmd.ExecuteNonQuery());

    //        /*SqlDataReader a = cmd.ExecuteReader();
    //        while (a.Read())
    //        {
    //            Console.WriteLine("{0}", a[0]);
    //        }
    //        Console.WriteLine("Success");*/

    //        genRandom(MyGlobal.sqlConnection1);
    //      0  //fillServiceMetricsTbl(MyGlobal.sqlConnection1);
    //        //fillDisplayColourTbl(sqlConnection1);
    //        //randomQueries(sqlConnection1);
    //       // Console.WriteLine("Success1");
    //        MyGlobal.sqlConnection1.Close();
    //       // Console.Read();
    //    }

    //    private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    //    {
            
    //    }

    //    static void genRandom(SqlConnection sql1)
    //    {
    //        SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
    //        cmd.Connection = sql1;
    //        string[] days = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    //        Random r = new Random();
    //        for (int i = 1; i <= 3; i++)
    //            for (int j = 1; j <= 2; j++)
    //            {
    //                for (int d = 0; d < 7; d++)
    //                    for (int h = 0; h < 24; h++)
    //                        for (int m = 0; m <= 50; m += 10)
    //                        {
    //                            if (m == 50)
    //                                cmd.CommandText = "insert into baseline_tbl values( \'" + days[d] + "\'" +
    //                                    ",\'" + h + ":" + m + ":" + "00.0000000\', \'" + (h + 1) % 24 + ":" + "00:00.0000000\'," + i + "," + j + "," + (r.Next(10000) + 10000) + ");";
    //                            else
    //                                cmd.CommandText = "insert into baseline_tbl values( \'" + days[d] + "\'" +
    //                                    ",\'" + h + ":" + m + ":" + "00.0000000\',\'" + h + ":" + (m + 10) + ":00.0000000\'," + i + "," + j + "," + (r.Next(10000) + 10000) + ");";
    //                            cmd.ExecuteNonQuery();
    //                            //Console.WriteLine(cmd.CommandText); 
    //                            //cmd.ExecuteReader();
    //                            //Console.WriteLine(cmd.CommandText);
    //                            //Console.ReadLine();
    //                            //Environment.Exit(0);
    //                        }
    //            }
    //        Console.Read();
    //    }


    //    static void fillServiceMetricsTbl(SqlConnection sql1)
    //    {
    //        SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            
    //        cmd.Connection = sql1;
    //        DateTime cur = new DateTime(2014, 5, 30);
    //        Random r = new Random();
    //        for (int i = 1; i <= 3; i++)
    //            for (int j = 1; j <= 2; j++)
    //            {
    //                cur = new DateTime(2014, 5, 31);

    //                for (int t = 0; t < 2880; t++)
    //                {
    //                    cmd.CommandText = "insert into service_metrics_tbl values (" + i + "," + j + ",\'" + cur.ToString() + "\'," + r.Next(20000) + ");";
    //                    //Console.WriteLine(cmd.CommandText);
    //                    cmd.ExecuteNonQuery();
    //                    cur = cur.AddMinutes(1);
    //                }
    //            }
    //    }

        
        //public static void fillRandomDataintoCollectedData()
        //{
        //    Random r = new Random();


        //    using (SqlConnection sqlConnection1 = new SqlConnection(MyGlobal.connstring))
        //    {
        //        sqlConnection1.Open();
        //        DateTime dt = new DateTime();






        //        //int service_id = 1;
        //        int metric_id = 1;
        //        bool flag = false;

        //        for (int s_id = 1; s_id < 6; s_id++)
        //        {
        //            int baseline = 1000;
        //            int per = 50;
        //            dt = DateTime.Now;
        //            dt = dt.AddDays(0);
        //            int value = (per * baseline) / 100;
        //            for (int i = 0; i < 10; i++)
        //            {

        //                for (int j = 0; j < (24 * 12); j++)
        //                {
        //                    Debug.WriteLine(dt);
        //                    value = (per * baseline) / 100;

        //                    SqlCommand cmd = new SqlCommand("BAM_AddDatatoCollectedData_prc", sqlConnection1);
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    //cmd.Parameters.Add(new SqlParameter("@day", SqlDbType.NChar)).Value = days[i];
        //                    cmd.Parameters.Add(new SqlParameter("@start_time", SqlDbType.DateTime)).Value = dt;
        //                    cmd.Parameters.Add(new SqlParameter("@end_time", SqlDbType.DateTime)).Value = dt.AddMinutes(5);
        //                    cmd.Parameters.Add(new SqlParameter("@s_id", SqlDbType.Int)).Value = s_id;
        //                    cmd.Parameters.Add(new SqlParameter("@m_id", SqlDbType.Int)).Value = metric_id;
        //                    cmd.Parameters.Add(new SqlParameter("@value", SqlDbType.Int)).Value = value;
        //                    cmd.Parameters.Add(new SqlParameter("@baseline", SqlDbType.Int)).Value = baseline;
        //                    cmd.Parameters.Add(new SqlParameter("@percentage", SqlDbType.Float)).Value = per;

        //                    cmd.ExecuteNonQuery();
        //                    if (j % 12 == 0)
        //                    {

        //                        baseline += (r.Next(-50, 50));
        //                        if (baseline < 0)
        //                            baseline = 0;
        //                    }
        //                    per += (r.Next(-5, 5));
        //                    if (per < 0)
        //                        per = 0;

        //                    if (r.Next(50) == 1)
        //                    {
        //                        per = 10;
        //                    }
        //                    if (r.Next(50) > 30 && per < 20)
        //                    {
        //                        per = 80;
        //                    }
        //                    if (!flag)
        //                    {

        //                        if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
        //                        {
        //                            flag = true;
        //                            baseline = 200;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
        //                        {
        //                            flag = false;
        //                            baseline = 1000;
        //                        }

        //                    }
        //                    dt = dt.AddMinutes(5);
        //                }


        //            }

        //        }
        //    }

        //}





    //    private void fillRandomDataintoBaseline()
    //    {
    //        Random r = new Random();

    //        int baseline = 2000;

    //        MyGlobal.sqlConnection1.Open();
    //        DateTime dt = new DateTime();
    //        //
            

    //        int service_id = 1;
    //        int metric_id = 2;

    //        string[] days = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    //        for (int i = 5; i < 7; i++)
    //        {

    //            for (int j = 0; j < (24 * 12); j++)
    //            {
    //                SqlCommand cmd = new SqlCommand("BAM_AddBaseline_prc", MyGlobal.sqlConnection1);
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.Parameters.Add(new SqlParameter("@day", SqlDbType.NChar)).Value = days[i];
    //                cmd.Parameters.Add(new SqlParameter("@start_time", SqlDbType.Time)).Value = dt.TimeOfDay;
    //                cmd.Parameters.Add(new SqlParameter("@end_time", SqlDbType.Time)).Value = dt.AddMinutes(5).TimeOfDay;
    //                cmd.Parameters.Add(new SqlParameter("@service_id", SqlDbType.Int)).Value = service_id;
    //                cmd.Parameters.Add(new SqlParameter("@metric_id", SqlDbType.Int)).Value = metric_id;
    //                cmd.Parameters.Add(new SqlParameter("@baseline", SqlDbType.Int)).Value = (baseline-(baseline%100));

    //                cmd.ExecuteNonQuery();
    //                baseline += (r.Next(-50,50)) ;
    //                if (baseline < 0)
    //                    baseline = 0;

                    
    //                dt = dt.AddMinutes(5);
    //            }


    //        }
            


    //        MyGlobal.sqlConnection1.Close();
}



