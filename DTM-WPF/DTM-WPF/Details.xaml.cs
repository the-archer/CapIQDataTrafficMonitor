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

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for Details.xaml
    /// </summary>
    public partial class Details : UserControl
    {
        public Details(int service_id)
        {
            InitializeComponent();

            DateTime end = DateTime.Now;
            DateTime start = DateTime.Now;
            start = start.AddDays(-1);
            UpdateGraph(service_id, "Processed", start, end);


        }

        public void UpdateGraph(int service_id, string metric_name, DateTime start, DateTime end)
        {

            TimeSpan interval = new TimeSpan(0, 0, 0, 0, (int)((end - start).TotalMilliseconds / 20));
            int metric_id = GetMetricID(metric_name);

            

                
        }

        private int GetMetricID(string metric_name)
        {
            throw new NotImplementedException();
        }
    }
}
