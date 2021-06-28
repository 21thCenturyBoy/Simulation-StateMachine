using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCode
{
    public interface IBehaviour
    {
        void Start();
        void Update(float deltatime);
    }

    public class Program
    {
        private static Task _updateTask;
        private static int _deltaTick = 20;

        private static IBehaviour exampleBehaviour;
        public static void Main(string[] args)
        {
            Task task = new Task(OnUpdate);
            task.Start();

            exampleBehaviour  = new Example_01();
            exampleBehaviour.Start();

            Console.ReadLine();
        }

        public static void OnUpdate()
        {
            while (true)
            {
                Task.Delay(20).Wait();
                exampleBehaviour?.Update(_deltaTick);
            }
        }
    }
}
