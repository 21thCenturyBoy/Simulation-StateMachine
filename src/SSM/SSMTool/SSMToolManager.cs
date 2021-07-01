using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMTool.Model;
using System.Text.RegularExpressions;

namespace SSMTool
{
    public enum FixedNameType
    {
        FSMName
    }

    public static class SSMToolManager
    {
        public static string m_GenerateFolderPath = @"GenerateFolder/CSharp/";//生成文件夹名
        public static string m_DgmlTemplatesFolder = @"DgmlTemplatesFolder/";
        public static string m_ScriptsTemplatePath = @"ScriptsTemplate.txt";//生成文件夹名

        #region Identification 标识

        public const string FSMContainerClassTypeID = "<#FSMContainerClassType#>";
        public const string FSMClassTypeID = "<#FSMClassType#>";
        public const string FSMNameID = "<#FSMName#>";
        public const string FiniteStateConstructorID = "<#FiniteStateConstructor#>";
        public const string FiniteStateLinkID = "<#FiniteStateLink#>";
        public const string FiniteStateConditionMethodID = "<#FiniteStateConditionMethod#>";
        public const string DateTimeID = "<#DateTime#>";

        public const string FSMClassType = "FSM";

        public static bool autoFixedName = true; //自动修复命名，不符合命名规范自动修复
        #endregion



        public static void GenerateCSharpScript()
        {
            CheckFileDirectory(m_GenerateFolderPath);
            CheckFileDirectory(m_DgmlTemplatesFolder);

            Dictionary<string, DirectedGraph> directedGraphs = GetDirectedGraphs();

            if (directedGraphs == null) return;//路径下没有dgml文件

            GenerateScripts(directedGraphs);
        }
        /// <summary>
        /// 检查目录
        /// </summary>
        /// <param name="path"></param>
        private static void CheckFileDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        /// <summary>
        /// 得到定向图字典
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, DirectedGraph> GetDirectedGraphs()
        {
            if (Directory.Exists(m_DgmlTemplatesFolder))
            {
                DirectoryInfo TheFolder = new DirectoryInfo(m_DgmlTemplatesFolder);
                //遍历文件
                Dictionary<string, DirectedGraph> dgmlfileDict = new Dictionary<string, DirectedGraph>();

                foreach (FileInfo dgmlfile in TheFolder.GetFiles("*.dgml"))
                {
                    dgmlfileDict.Add(dgmlfile.Name, DirectedGraphExtensions.ReadFormFile(dgmlfile.FullName));
                }
                return dgmlfileDict;
            }
            return null;
        }

        private static void GenerateScripts(Dictionary<string, DirectedGraph> directedGraphs)
        {
            foreach (var directedGraph in directedGraphs)
            {
                try
                {
                    string fsmName = directedGraph.Key.Split('.')[0];

                    bool res = fsmName.RegexMatchName();

                    if (!res)//状态机命名不符合命名规范需要修复
                    {
                        if (autoFixedName) fsmName = fsmName.FixedName();
                        else throw new Exception("不符合命名规范需要修复");
                    }
                    GenerateScript(fsmName, directedGraph.Value);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString(), ConsoleColor.Red);
                    throw;
                }
            }
        }

        private static void GenerateScript(string name, DirectedGraph graph)
        {
            try
            {
                string FSMContainerClassType = name + "Container";

                string path = m_GenerateFolderPath + FSMContainerClassType + ".cs";

                FileInfo template = new FileInfo(m_ScriptsTemplatePath);

                using (var temp = template.OpenRead())
                {
                    using (StreamReader reader = new StreamReader(temp, Encoding.UTF8))
                    {
                        string str = reader.ReadToEnd();

                        str = ReplaceString(str, name, graph);

                        using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            using (StreamWriter writer = new StreamWriter(file))
                            {
                                writer.Write(str);
                            }
                        }
                    }
                }
                Debug.Log(path + "   文件已生成！");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), ConsoleColor.Red);
                throw;
            }
        }

        /// <summary>
        /// 替换字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="FSMName"></param>
        /// <param name="dgmlfile"></param>
        /// <returns></returns>
        private static string ReplaceString(string str, string FSMName, DirectedGraph dgmlfile)
        {
            StringBuilder sb = new StringBuilder(str);

            string fsmContainerClassType = FSMName + "Container";
            sb.Replace(FSMNameID, FSMName).Replace(FSMContainerClassTypeID, fsmContainerClassType).Replace(FSMClassTypeID, FSMClassType).Replace(DateTimeID,DateTime.Now.ToString());

            Dictionary<Node, string> fixedNameDict;
            sb.Replace(FiniteStateConstructorID, GetFiniteStateConstructorString(dgmlfile, out fixedNameDict));

            List<string> conditionMethodList;
            sb.Replace(FiniteStateLinkID, GetFiniteStateLinkString(dgmlfile, fixedNameDict, out conditionMethodList));

            sb.Replace(FiniteStateConditionMethodID,GetFiniteStateConditionMethodString(conditionMethodList));
            return sb.ToString();
        }

        /// <summary>
        /// 替换状态构造机
        /// </summary>
        /// <param name="dgmlfile"></param>
        /// <returns></returns>
        private static string GetFiniteStateConstructorString(DirectedGraph dgmlfile, out Dictionary<Node, string> fixedNameDict)
        {
            fixedNameDict = new Dictionary<Node, string>();

            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (Node node in dgmlfile.Nodes)
                {
                    bool isMatch = node.Id.RegexMatchName();

                    if (!isMatch && !autoFixedName) throw new Exception("不符合命名规范需要修复");

                    string fixedname = isMatch ? node.Id : node.Id.FixedName();

                    fixedNameDict.Add(node, fixedname);

                    sb.AppendLine($"\t\tFiniteState {fixedname} = new FiniteState(\"{node.Label ?? node.Id}\");");
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString(), ConsoleColor.Red);
                throw;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成状态机自动连线
        /// </summary>
        /// <returns></returns>
        private static string GetFiniteStateLinkString(DirectedGraph dgmlfile, Dictionary<Node, string> nameDictionary, out List<string> methodNameList)
        {
            methodNameList = new List<string>();
            StringBuilder sb = new StringBuilder();

            foreach (Link link in dgmlfile.Links)
            {
                string sourceName = nameDictionary[dgmlfile.GetNode(link.Source)];
                string targetName = nameDictionary[dgmlfile.GetNode(link.Target)];
                string method = $"{sourceName}To{targetName}";

                sb.AppendLine($"\t\t{sourceName}.To({targetName},{method});");
                methodNameList.Add(method);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成状态机跳转条件
        /// </summary>
        /// <returns></returns>
        private static string GetFiniteStateConditionMethodString(List<string> conditionMethodList)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < conditionMethodList.Count; i++)
            {
                //        private bool IsOnGroundState(OnGroundStateEnum targetState) => targetState == onGroundState;
                sb.AppendLine($"\tprivate bool {conditionMethodList[i]}()\n\t{{\n\t\treturn false;\n\t}}");
            }

            return sb.ToString();
        }
    }
}
