using System;
using SSM;

namespace ExampleCode
{
    public class Example_02 : FSMBehaviour
    {
        public enum OnGroundStateEnum
        {
            no,
            run,
            walk,
            stand
        }

        public bool jumpFlag;
        public OnGroundStateEnum onGroundState;

        public FSM PlayerSimpleFSM;
        public override void Start()
        {
            base.Start();

            InitFSM();
            Console.WriteLine("当前状态:{0},输入指令:{1}，{2}，{3}，{4}", PlayerSimpleFSM.CurrentState.Name,"w","s","d","dd");
        }
        public override void Update(float deltatime)
        {
            base.Update(deltatime);

            PlayerSimpleFSM.Tick();

        }
        public override void GetInput(string inputOrder)
        {
            base.GetInput(inputOrder);
            switch (inputOrder)
            {
                case "w":
                    jumpFlag = true;
                    onGroundState = OnGroundStateEnum.no;
                    break;
                case "s":
                    onGroundState = OnGroundStateEnum.stand;
                    break;
                case "d":
                    onGroundState = OnGroundStateEnum.walk;
                    break;
                case "dd":
                    onGroundState = OnGroundStateEnum.run;
                    break;
                default: break;
            }
        }
        private void InitFSM()
        {
            PlayerSimpleFSM = new FSM();

            HierarchicalFS OnGroundState = new HierarchicalFS("在地面状态");
            HierarchicalFS runState = new HierarchicalFS("奔跑状态");
            HierarchicalFS walkState = new HierarchicalFS("行走状态");
            HierarchicalFS standState = new HierarchicalFS("站立状态");
            TimerFS jumpState = new TimerFS("跳跃状态", new TimeChunk(GetDeltaTime, 500), null, () => { jumpFlag = false; });

            OnGroundState.AddChildren(standState,runState, walkState);

            jumpState.To(OnGroundState, jumpState.Finished).To(jumpState, () => jumpFlag);
            runState.OrTo(walkState, () => IsOnGroundState(OnGroundStateEnum.walk))
                .OrTo(standState, () => IsOnGroundState(OnGroundStateEnum.stand));
            walkState.OrTo(runState, () => IsOnGroundState(OnGroundStateEnum.run))
                .OrTo(standState, () => IsOnGroundState(OnGroundStateEnum.stand));
            standState.OrTo(runState, () => IsOnGroundState(OnGroundStateEnum.run))
                .OrTo(walkState, () => IsOnGroundState(OnGroundStateEnum.walk));

            PlayerSimpleFSM.SetState(standState);

            PlayerSimpleFSM.StateChangeEvent += PlayerSimpleFSM_StateChangeEvent;
        }

        private bool IsOnGroundState(OnGroundStateEnum targetState) => targetState == onGroundState;
        private void PlayerSimpleFSM_StateChangeEvent(IState arg1, IState arg2)
        {
            Console.WriteLine(arg1.Name + "-->" + arg2.Name);
        }
    }
}
