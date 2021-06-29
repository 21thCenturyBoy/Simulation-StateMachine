using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleCode
{
    public class FSMBehaviour : IBehaviour
    {
        private float _deltaTime;
        public float GetDeltaTime() => _deltaTime;
        public virtual void Start() { }
        public virtual void Update(float deltatime)
        {
            _deltaTime = deltatime;
        }

        public virtual void GetInput(string inputOrder) { }
    }
    public interface IBehaviour
    {
        void Start();
        void Update(float deltatime);
        void GetInput(string inputOrder);
    }

    public class Program
    {
        private static Task _updateTask;
        private static int _deltaTick = 20;

        private static IBehaviour exampleBehaviour;
        public static void Main(string[] args)
        {

            ////TODO 取消注释运行---展示了一个预警灯由通电到烧断的例子
            //exampleBehaviour = new Example_01();
            //exampleBehaviour.Start();

            ////TODO 取消注释运行---展示了一个玩家控制角色的例子
            //exampleBehaviour = new Example_02();
            //exampleBehaviour.Start();

            Task task = new Task(OnUpdate);
            task.Start();

            OnInput();
        }

        public static void OnUpdate()
        {
            while (true)
            {
                Task.Delay(_deltaTick).Wait();
                exampleBehaviour?.Update(_deltaTick);
            }
        }

        public static void OnInput()
        {
            while (true)
            {
                exampleBehaviour.GetInput(Console.ReadLine());
            }
        }
    }
}
