using System;
using System.Collections;
using System.Collections.Generic;

namespace SSM
{
    public interface IStateMachine
    {
        IState To(IState from, IState to);
        IState To(IState from, IState to, Func<bool> condition);

        void SetState(IState state);
        void Tick();
    }


    public class FSM : IStateMachine
    {

        protected IState _currentState;
        public IState CurrentState { get => _currentState; }
        public IState To(IState from, IState to) { from.To(to); return to; }

        public IState To(IState from, IState to, Func<bool> condition) { from.To(to, condition); return to; }

        public int BatchAddState(Dictionary<FiniteState, Func<bool>> batchsData)
        {
            int counter = 0;
            //           b's condition        a's condition
            //eg:    a---------------->b---------------->a
            foreach (FiniteState a in batchsData.Keys)
            {
                foreach (KeyValuePair<FiniteState, Func<bool>> conditionBatchse in batchsData)
                {
                    var b = conditionBatchse.Key;
                    if (a.Equals(b)) break;

                    a.To(b, conditionBatchse.Value);
                    counter++;
                }
            }
            return counter;
        }

        public void SetState(IState state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public virtual void Tick()
        {
            _currentState.Tick();
            IState to = _currentState.GetTransitionState();
            if (to != null) SetState(to);
        }
    }
    public class HFSM : FSM
    {
        public override void Tick()
        {
            HierarchicalFiniteState current = (_currentState as HierarchicalFiniteState)?.GetFinalEntryState();
            if (current != null) SetState(_currentState);
            _currentState.Tick();
            IState to = _currentState.GetTransitionState();
            if (to != null) SetState(to);
        }
    }
}
