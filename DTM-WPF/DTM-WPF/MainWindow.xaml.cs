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
}



