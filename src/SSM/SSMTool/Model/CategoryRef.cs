using System.Xml.Serialization;

namespace SSMTool.Model
{
    public class CategoryRef
    {
        [XmlAttribute]
        public string Ref { get; set; }
    }
}
