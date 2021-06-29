using SSM;
using System;

namespace ExampleCode
{
    public class Example_01 : FSMBehaviour
    {
        public FSM LampFsm;

        public bool IsLight = false;//是否亮
        public bool IsPower = false;//是否通电

        public override void Start()
        {
            base.Start();

            InitFSM();
            Console.WriteLine("请输入命令:1");
        }

        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            LampFsm.Tick();
        }
        public override void GetInput(string inputOrder)
        {
            base.GetInput(inputOrder);

            if (inputOrder == "1") IsPower = true;
        }

        private void InitFSM()
        {
            LampFsm = new FSM();

            FiniteState idelState = new FiniteState("未通电状态");
            TimerFS brightState = new TimerFS("明亮状态", new TimeChunk(GetDeltaTime, 5000));
            TimerTaskFS flashState = new TimerTaskFS("闪烁状态", new TimeChunk(GetDeltaTime, 1000), () => { IsLight = !IsLight; }, 3);
            FiniteState blowoutState = new FiniteState("灯丝烧断状态");

            idelState.To(brightState, () => IsPower).To(flashState, brightState.Finished).To(blowoutState, flashState.Finished);

            LampFsm.SetState(idelState);

            LampFsm.StateChangeEvent += LampFsm_StateChangeEvent;
        }

        private void LampFsm_StateChangeEvent(IState arg1, IState arg2)
        {
            Console.WriteLine(arg1.Name + "-->" + arg2.Name);
        }


    }

}
