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
using QuickGraph;
using System.Diagnostics;

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
    }
}


namespace GraphDemo
{
    [DebuggerDisplay("{ID}-{IsMale}")]
    public class PocVertex
    {
        public string ID { get; private set; }
        public bool IsMale { get; private set; }

        public PocVertex(string id, bool isMale)
        {
            ID = id;
            IsMale = isMale;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", ID, IsMale);
        }
    }

    [DebuggerDisplay("{Source.ID} -> {Target.ID}")]
    public class PocEdge : Edge<PocVertex>
    {
        public string ID
        {
            get;
            private set;
        }

        public PocEdge(string id, PocVertex source, PocVertex target)
            : base(source, target)
        {
            ID = id;
        }
    }

    public class PocGraph : BidirectionalGraph<PocVertex, PocEdge>
    {
        public PocGraph() { }

        public PocGraph(bool allowParallelEdges)
            : base(allowParallelEdges) { }

        public PocGraph(bool allowParallelEdges, int vertexCapacity)
            : base(allowParallelEdges, vertexCapacity) { }
    }

    public class PocGraphLayout : GraphLayout<PocVertex, PocEdge, PocGraph> { }


}


