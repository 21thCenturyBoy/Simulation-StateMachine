using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SSMTool
{
    class Program
    {
        static void Main(string[] args)
        {
            SSMToolManager.GenerateCSharpScript();

            Debug.Log("Press any key to exit...");
            Console.ReadLine();
        }
    }

}
