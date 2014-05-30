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
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            InitializeComboBox();
           
          
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

    }
}
