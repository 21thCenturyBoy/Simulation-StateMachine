using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSM;

namespace ExampleCode
{
    public class Example_01 : IBehaviour
    {
        public FSM LampFsm;

        private float _deltaTime;

        public bool IsLight = false;//是否亮
        public bool IsPower = true;//是否通电

        public void Start()
        {
            InitFSM();
        }

        public void Update(float deltatime)
        {
            _deltaTime = deltatime;

            LampFsm.Tick();
        }

        private void InitFSM()
        {
            LampFsm = new FSM();

            FiniteState idelState = new FiniteState("未通电状态");
            TimerFS brightState = new TimerFS("明亮状态", new TimeChunk(() => _deltaTime, 5000));
            TimerTaskFS flashState = new TimerTaskFS("闪烁状态", new TimeChunk(() => _deltaTime, 1000), () => { IsLight = !IsLight; },3);
            FiniteState blowoutState = new FiniteState("灯丝烧断状态");

            idelState.To(brightState, ()=> IsPower).To(flashState,brightState.Finished).To(blowoutState,flashState.Finished);

            LampFsm.SetState(idelState);

            LampFsm.StateChangeEvent += LampFsm_StateChangeEvent;
        }

        private void LampFsm_StateChangeEvent(IState arg1, IState arg2)
        {
            Console.WriteLine(arg1.Name + "-->" + arg2.Name);
        }
    }
}
