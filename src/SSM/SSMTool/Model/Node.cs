using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SSMTool.Model
{
    public class Node : IEquatable<Node>
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Label { get; set; }
        [XmlAttribute]
        public string Category { get; set; }
        [XmlAttribute]
        public string Group { get; set; }
        [XmlAttribute]
        public string Description { get; set; }

        [XmlElement("Category")]
        public List<CategoryRef> CategoryRefs { get; set; } = new List<CategoryRef>();
        [XmlAttribute]
        public string Reference { get; set; }
        [XmlIgnore]
        public Dictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        #region Intereface

        public bool Equals(Node other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        public static bool operator ==(Node left, Node right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Node left, Node right)
        {
            return !Equals(left, right);
        }

        #endregion

    }
}
