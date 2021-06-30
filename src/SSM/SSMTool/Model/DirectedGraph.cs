using System.Collections.Generic;
using System.Xml.Serialization;

namespace SSMTool.Model
{
    [XmlRoot("DirectedGraph", Namespace = "http://schemas.microsoft.com/vs/2009/dgml")]
    public class DirectedGraph
    {
        public DirectedGraph()
        {
            Nodes = new List<Node>();
            Links = new List<Link>();
            Categories = new List<Category>();
        }
        
        public List<Node> Nodes { get; set; }
        public List<Link> Links { get; set; }
        public List<Category> Categories { get; set; }
    }

}
