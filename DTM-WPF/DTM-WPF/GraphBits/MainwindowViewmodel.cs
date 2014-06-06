using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using GraphSharp.Controls;
using System.Data.SqlClient;
using System.Diagnostics;


namespace DTM_WPF
{
    public class PocGraphLayout : GraphLayout<PocVertex, PocEdge, PocGraph> { }



    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Data

        private string layoutAlgorithmType;
        private PocGraph graph;
        private List<String> layoutAlgorithmTypes = new List<string>();
        private int count;
        #endregion

        #region Ctor
        public MainWindowViewModel(int metricId, DateTime time)
        {
            Graph = new PocGraph(true);

            ReLayoutGraph(metricId, time);

            //Add Layout Algorithm Types
            layoutAlgorithmTypes.Add("BoundedFR");
            layoutAlgorithmTypes.Add("Circular");
            layoutAlgorithmTypes.Add("CompoundFDP");
            layoutAlgorithmTypes.Add("EfficientSugiyama");
            layoutAlgorithmTypes.Add("FR");
            layoutAlgorithmTypes.Add("ISOM");
            layoutAlgorithmTypes.Add("KK");
            layoutAlgorithmTypes.Add("LinLog");
            layoutAlgorithmTypes.Add("Tree");

            //Pick a default Layout Algorithm Type
            LayoutAlgorithmType = "Tree";

        }
        #endregion


        public void ReLayoutGraph(int metricId, DateTime time)
        {
            graph = new PocGraph(true);
            count++;

            List<PocVertex> existingVertices = loadData(UserControl1.getStats(metricId,time));

            NotifyPropertyChanged("Graph");

        }

        public List<PocVertex> loadData(Dictionary<int, Tuple<string, int, int, float, string>> data)
        {
            List<PocVertex> existingVertices = new List<PocVertex>();

            var keySet = data.Keys;
            foreach (var key in keySet)
                existingVertices.Add(new PocVertex(data[key].Item1, key, data[key].Item4, data[key].Item5));

            foreach (PocVertex vertex in existingVertices)
                Graph.AddVertex(vertex);

            addEdges(existingVertices);
            return existingVertices;
        }

        public void addEdges(List<PocVertex> existingVertices)
        {
            Dictionary<int, int> serviceHash = new Dictionary<int, int>();
            for(int i=0;i<existingVertices.Count;i++)
            {
                var vertex = existingVertices[i];
                serviceHash.Add(vertex.serviceId, i);
            }


            MyGlobal.sqlConnection1.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from edges_tbl";
            cmd.Connection = MyGlobal.sqlConnection1;

            SqlDataReader reader=cmd.ExecuteReader();
            while (reader.Read())
            {
                //Debug.WriteLine();
                AddNewGraphEdge
                    (existingVertices[serviceHash[Convert.ToInt32(reader[0])]], existingVertices[serviceHash[Convert.ToInt32(reader[1])]]);
            }
            reader.Close();
            MyGlobal.sqlConnection1.Close();
        }



        #region Private Methods
        private PocEdge AddNewGraphEdge(PocVertex from, PocVertex to)
        {
            string edgeString = string.Format("{0}-{1} Connected", from.serviceName, to.serviceName);

            PocEdge newEdge = new PocEdge(edgeString, from, to);
            Graph.AddEdge(newEdge);
            return newEdge;
        }


        #endregion

        #region Public Properties

        public List<String> LayoutAlgorithmTypes
        {
            get { return layoutAlgorithmTypes; }
        }


        public string LayoutAlgorithmType
        {
            get { return layoutAlgorithmType; }
            set
            {
                layoutAlgorithmType = value;
                NotifyPropertyChanged("LayoutAlgorithmType");
            }
        }



        public PocGraph Graph
        {
            get { return graph; }
            set
            {
                graph = value;
                NotifyPropertyChanged("Graph");
            }
        }
        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
