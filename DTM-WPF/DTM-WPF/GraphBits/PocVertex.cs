using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DTM_WPF{
    /// <summary>
    /// A simple identifiable vertex.
    /// </summary>
    [DebuggerDisplay("{serviceName}")]
    public class PocVertex
    {
        public int serviceId { get; set; }
        public string serviceName { get; set; }
        public float perc { get; set; }
        public string color { get; set; }

        public PocVertex(string servicename, int serviceid, float p, string Color)
        {
            serviceName = servicename;
            serviceId = serviceid;
            perc = p;
            color = Color;
        }

        public override string ToString()
        {
            return string.Format("{0}", serviceName);
        }
    }
}
