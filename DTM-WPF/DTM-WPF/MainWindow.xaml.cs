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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            DTMClass dtmclass = new DTMClass();

            //caller();
            
             Dictionary<int, Tuple<string, int, int, double, string>> data = dtmclass.getStats(DateTime.Now);
        }

        private void caller()
        {
            Console.Write("Hello sd!");

            SqlConnection sqlConnection1 =
              new System.Data.SqlClient.SqlConnection(@"Data Source=CIQGUR-ATD133\SQLEXPRESS;Initial Catalog=dtm;Integrated Security=True");


            SqlCommand cmd = new System.Data.SqlClient.SqlCommand();

            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.Parameters.AddWithValue("@id1", 10);
            //cmd.Parameters.AddWithValue("@name1", "hello");
            //cmd.CommandText = "insert into baseline_tbl values ('Monday','0:0:00.0000000','0:00:0.0000000',1,1,1000);";
            //cmd.CommandText = "select count(*) from display_colour_tbl";
            //cmd.CommandText = "select * from service_metrics_tbl; ";
            cmd.Connection = sqlConnection1;
            Console.WriteLine("Success");
            sqlConnection1.Open();
            Console.WriteLine("Success");
            //Console.Write(cmd.ExecuteNonQuery());

            /*SqlDataReader a = cmd.ExecuteReader();
            while (a.Read())
            {
                Console.WriteLine("{0}", a[0]);
            }
            Console.WriteLine("Success");*/

            //genRandom(sqlConnection1);
            fillServiceMetricsTbl(sqlConnection1);
            //fillDisplayColourTbl(sqlConnection1);
            //randomQueries(sqlConnection1);
           // Console.WriteLine("Success1");
            sqlConnection1.Close();
           // Console.Read();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        static void fillServiceMetricsTbl(SqlConnection sql1)
        {
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = sql1;
            DateTime cur = new DateTime(2014, 5, 30);
            Random r = new Random();
            for (int i = 1; i <= 3; i++)
                for (int j = 1; j <= 2; j++)
                {
                    cur = new DateTime(2014, 5, 30);

                    for (int t = 0; t < 2880; t++)
                    {
                        cmd.CommandText = "insert into service_metrics_tbl values (" + i + "," + j + ",\'" + cur.ToString() + "\'," + r.Next(20000) + ");";
                        //Console.WriteLine(cmd.CommandText);
                        cmd.ExecuteNonQuery();
                        cur = cur.AddMinutes(1);
                    }
                }
        }



    }
}
