using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMTool
{
    /// <summary>
    /// 日志输出管理
    /// </summary>
    public static  class Debug
    {
        public static void Log(string info)
        {
            Log(info, ConsoleColor.White);
        }
        public static void Log(string info, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(info);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
