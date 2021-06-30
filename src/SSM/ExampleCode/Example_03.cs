using System;
using System.Collections.Generic;
using SSM;

namespace ExampleCode
{
    public class Example_03 : FSMBehaviour
    {
        public FSM ComplexFSM;
        public override void Start()
        {
            base.Start();

            InitFSM();
            Console.WriteLine("当前状态：{0}\n输入指令：A=值，B=值，C=值切换状态，例如：A=2：",ComplexFSM.CurrentState.Name);
        }
        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            ComplexFSM.Tick();

        }

        private int A_Val;
        private int B_Val;
        private int C_Val;

        private void InitFSM()
        {

            ComplexFSM = new FSM();

            TagPair A1tag = new TagPair(0, 0);
            TagPair A2tag = new TagPair(0, 1);
            TagPair A3tag = new TagPair(0, 2);
            TagPair B1tag = new TagPair(1, 0);
            TagPair B2tag = new TagPair(1, 1);
            TagPair C1tag = new TagPair(2, 0);
            TagPair C2tag = new TagPair(2, 1);

            Dictionary<TagPair, Func<bool>> stateDic = new Dictionary<TagPair, Func<bool>>();
            stateDic.Add(A1tag, () => A_Val == 0);
            stateDic.Add(A2tag, () => A_Val == 1);
            stateDic.Add(A3tag, () => A_Val == 2);
            stateDic.Add(B1tag, () => B_Val == 0);
            stateDic.Add(B2tag, () => B_Val == 1);
            stateDic.Add(C1tag, () => C_Val == 0);
            stateDic.Add(C2tag, () => C_Val == 1);

            StatePoint a1b1C1Point = new StatePoint(new FiniteState("A1B1C1"), A1tag, B1tag, C1tag);
            StatePoint a2b1C1Point = new StatePoint(new FiniteState("A2B1C1"), A2tag, B1tag, C1tag);
            StatePoint a3b1C1Point = new StatePoint(new FiniteState("A3B1C1"), A3tag, B1tag, C1tag);
            StatePoint a1b1C2Point = new StatePoint(new FiniteState("A1B1C2"), A1tag, B1tag, C2tag);
            StatePoint a2b1C2Point = new StatePoint(new FiniteState("A2B1C2"), A2tag, B1tag, C2tag);
            StatePoint a3b1C2Point = new StatePoint(new FiniteState("A3B1C2"), A3tag, B1tag, C2tag);
            StatePoint a1b2C1Point = new StatePoint(new FiniteState("A1B2C1"), A1tag, B2tag, C1tag);
            StatePoint a2b2C1Point = new StatePoint(new FiniteState("A2B2C1"), A2tag, B2tag, C1tag);
            StatePoint a3b2C1Point = new StatePoint(new FiniteState("A3B2C1"), A3tag, B2tag, C1tag);
            StatePoint a1b2C2Point = new StatePoint(new FiniteState("A1B2C2"), A1tag, B2tag, C2tag);
            StatePoint a2b2C2Point = new StatePoint(new FiniteState("A2B2C2"), A2tag, B2tag, C2tag);
            StatePoint a3b2C2Point = new StatePoint(new FiniteState("A3B2C2"), A3tag, B2tag, C2tag);

            StateMap stateMap = new StateMap(stateDic);

            stateMap.AddStatePoint(a1b1C1Point, a2b1C1Point, a3b1C1Point, a1b1C2Point, a2b1C2Point, a3b1C2Point, a1b2C1Point, a2b2C1Point, a3b2C1Point, a1b2C2Point, a2b2C2Point, a3b2C2Point);
            if (stateMap.CreateStateMap())stateMap.CreateStateFSM();

            ComplexFSM.SetState(a1b1C1Point.State);

            ComplexFSM.StateChangeEvent += ComplexFSM_StateChangeEvent;
        }

        private void ComplexFSM_StateChangeEvent(IState arg1, IState arg2)
        {
            Console.WriteLine(arg1.Name + "-->" + arg2.Name);
        }

        public override void GetInput(string inputOrder)
        {
            switch (inputOrder)
            {
                case "A=1": A_Val = 0; break;
                case "A=2": A_Val = 1; break;
                case "A=3": A_Val = 3; break;
                case "B=1": B_Val = 0; break;
                case "B=2": B_Val = 1; break;
                case "C=1": C_Val = 0; break;
                case "C=2": C_Val = 1; break;
                default: break;
            }
        }
    }
}
