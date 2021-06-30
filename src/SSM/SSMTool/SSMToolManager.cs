using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMTool.Model;

namespace SSMTool
{
    public static class SSMToolManager
    {
        public static string m_GenerateFolderPath = @"GenerateFolder/CSharp/";//生成文件夹名
        public static string m_DgmlTemplatesFolder = @"DgmlTemplatesFolder/";
        public static string m_ScriptsTemplatePath = @"ScriptsTemplate.txt";//生成文件夹名




        public static void GenerateCSharpScript()
        {
            CheckFileDirectory(m_GenerateFolderPath);
            CheckFileDirectory(m_DgmlTemplatesFolder);

            Dictionary<string, DirectedGraph> directedGraphs = GetDirectedGraphs();

            if (directedGraphs == null) ConsoleLog("错误", ConsoleColor.Red);

            foreach (var directedGraph in directedGraphs)
            {
                ConsoleLog("个数：" + directedGraph.Value.Nodes.Count);
                try
                {
                    string fsm = directedGraph.Key.Split('.')[0];
                    GenerateScripts(fsm, directedGraph.Value);
                }
                catch (Exception e)
                {
                    ConsoleLog(e.ToString(), ConsoleColor.Red);
                    throw;
                }

            }
        }
        private static void CheckFileDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        private static Dictionary<string, DirectedGraph> GetDirectedGraphs()
        {
            if (Directory.Exists(m_DgmlTemplatesFolder))
            {
                DirectoryInfo TheFolder = new DirectoryInfo(m_DgmlTemplatesFolder);
                //遍历文件
                Dictionary<string, DirectedGraph> list = new Dictionary<string, DirectedGraph>();
                foreach (FileInfo NextFile in TheFolder.GetFiles())
                {
                    list.Add(NextFile.Name, DirectedGraphExtensions.ReadFormFile(NextFile.FullName));
                }
                return list;
            }

            return null;
        }

        private static void GenerateScripts(string FSMName, DirectedGraph dgmlfile)
        {
            try
            {
                string FSMContainerClassType = FSMName + "Container";

                string path = m_GenerateFolderPath + FSMContainerClassType + ".cs";

                FileInfo template = new FileInfo(m_ScriptsTemplatePath);

                using (var temp = template.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(temp, Encoding.UTF8))
                    {
                        string str = reader.ReadToEnd();

                        str = str.Replace("<#FSMContainerClassType#> ", FSMContainerClassType);


                        using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            using (StreamWriter writer = new StreamWriter(file))
                            {
                                writer.Write(str);
                            }
                            ConsoleLog(str.Length + "已写入！");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleLog(e.ToString(), ConsoleColor.Red);
            }
        }
        public static void ConsoleLog(string info)
        {
            ConsoleLog(info, ConsoleColor.White);
        }
        public static void ConsoleLog(string info, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(info);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
