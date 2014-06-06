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
using GraphSharp.Controls;
using DTM_WPF.GraphBits;

namespace DTM_WPF
{



    public partial class MainWindow1 : Window
    {
        private MainWindowViewModel vm;
        public MainWindow1(int metricId, DateTime time)
        {
            vm = new MainWindowViewModel(metricId, time);
            this.DataContext = vm;
        }



        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    vm.ReLayoutGraph();
        //}
    }
}
