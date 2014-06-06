﻿using System;
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
            Debug.WriteLine("Before init");
            InitializeComponent();

                //caller();
        }

   

        private void caller()
        {
            Debug.Write("Hello sd!");





            MyGlobal.sqlConnection1.Open();
            //cmd.CommandType = System.Data.CommandType.Text;
            //cmd.Parameters.AddWithValue("@id1", 10);
            //cmd.Parameters.AddWithValue("@name1", "hello");
            //cmd.CommandText = "insert into baseline_tbl values ('Monday','0:0:00.0000000','0:00:0.0000000',1,1,1000);";
            //cmd.CommandText = "select count(*) from display_colour_tbl";
            //cmd.CommandText = "select * from service_metrics_tbl; ";
           
            Debug.WriteLine("Success");
            
            Debug.WriteLine("Success");
            //Console.Write(cmd.ExecuteNonQuery());

            /*SqlDataReader a = cmd.ExecuteReader();
            while (a.Read())
            {
                Console.WriteLine("{0}", a[0]);
            }
            Console.WriteLine("Success");*/

            genRandom(MyGlobal.sqlConnection1);
            //fillServiceMetricsTbl(MyGlobal.sqlConnection1);
            //fillDisplayColourTbl(sqlConnection1);
            //randomQueries(sqlConnection1);
           // Console.WriteLine("Success1");
            MyGlobal.sqlConnection1.Close();
           // Console.Read();
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        static void genRandom(SqlConnection sql1)
        {
            SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = sql1;
            string[] days = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            Random r = new Random();
            for (int i = 1; i <= 3; i++)
                for (int j = 1; j <= 2; j++)
                {
                    for (int d = 0; d < 7; d++)
                        for (int h = 0; h < 24; h++)
                            for (int m = 0; m <= 50; m += 10)
                            {
                                if (m == 50)
                                    cmd.CommandText = "insert into baseline_tbl values( \'" + days[d] + "\'" +
                                        ",\'" + h + ":" + m + ":" + "00.0000000\', \'" + (h + 1) % 24 + ":" + "00:00.0000000\'," + i + "," + j + "," + (r.Next(10000) + 10000) + ");";
                                else
                                    cmd.CommandText = "insert into baseline_tbl values( \'" + days[d] + "\'" +
                                        ",\'" + h + ":" + m + ":" + "00.0000000\',\'" + h + ":" + (m + 10) + ":00.0000000\'," + i + "," + j + "," + (r.Next(10000) + 10000) + ");";
                                cmd.ExecuteNonQuery();
                                //Console.WriteLine(cmd.CommandText); 
                                //cmd.ExecuteReader();
                                //Console.WriteLine(cmd.CommandText);
                                //Console.ReadLine();
                                //Environment.Exit(0);
                            }
                }
            Console.Read();
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
                    cur = new DateTime(2014, 5, 31);

                    for (int t = 0; t < 2880; t++)
                    {
                        cmd.CommandText = "insert into service_metrics_tbl values (" + i + "," + j + ",\'" + cur.ToString() + "\'," + r.Next(20000) + ");";
                        //Console.WriteLine(cmd.CommandText);
                        cmd.ExecuteNonQuery();
                        cur = cur.AddMinutes(1);
                    }
                }
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            //Live livepage = new Live();



            contentControl1.Content = new UserControl1();

            //DTMClass dtmclass = new DTMClass();



            //caller();

           // Dictionary<int, Tuple<string, int, int, double, string>> data = dtmclass.getStats(DateTime.Now);

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            contentControl1.Content = new Analysis();
        }

       

      


    }

    public class MyGlobal
    {
        public static string connstring = @"Data Source=CIQGUR-ATD133\sqlexpress;Initial Catalog=dtm;Integrated Security=True;Pooling=False";
        public static System.Data.SqlClient.SqlConnection sqlConnection1 =
              new System.Data.SqlClient.SqlConnection(MyGlobal.connstring);
    }
}



