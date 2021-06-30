using System.IO;
using System.Xml.Serialization;

namespace SSMTool.Model
{
    public static class DirectedGraphExtensions
    {
        /// <summary>
        /// 写入dgml
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="fileName"></param>
        public static void WriteToFile(this DirectedGraph graph, string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var writer = new StreamWriter(file, System.Text.Encoding.UTF8))
                {
                    var serializer = new XmlSerializer(typeof(DirectedGraph));
                    serializer.Serialize(writer, graph);
                }
            }
        }
        /// <summary>
        /// 读取dgml
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DirectedGraph ReadFormFile(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(typeof(DirectedGraph));
                return (DirectedGraph)serializer.Deserialize(file);
            }
        }
    }
}
