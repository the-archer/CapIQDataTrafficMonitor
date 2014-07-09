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
using System.ComponentModel;

namespace DTM_WPF
{
    /// <summary>
    /// Interaction logic for MyProgressBar.xaml
    /// </summary>
    public partial class MyProgressBar : UserControl,INotifyPropertyChanged
    {
        public MyProgressBar()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += new RoutedEventHandler(MyProgressBar_Loaded);
        }

        void MyProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        public static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(MyProgressBar), new PropertyMetadata(100d, OnMaximumChanged));
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        public static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(MyProgressBar), new PropertyMetadata(0d, OnMinimumChanged));
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(MyProgressBar), new PropertyMetadata(0d, OnValueChanged));
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty,value); }
        }



        public static DependencyProperty ProgressBarWidthProperty = DependencyProperty.Register("ProgressBarWidth", typeof(double), typeof(MyProgressBar), null);
        public double ProgressBarWidth
        {
            get { return (double)GetValue(ProgressBarWidthProperty); }
            set { SetValue(ProgressBarWidthProperty, value); }
        }

        static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressBar).Update();
        }

        static void OnMinimumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressBar).Update();
        }

        static void OnMaximumChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as MyProgressBar).Update();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        void Update()
        {
            // The -2 is for the borders - there are probably better ways of doing this since you
            // may want your template to have variable bits like border width etc which you'd use
            // TemplateBinding for
            ProgressBarWidth = Math.Min((Value / (Maximum + Minimum) * this.ActualWidth) - 2, this.ActualWidth - 2);
            NotifyPropertyChanged("ProgressBarWidth");

        }
    }
}
