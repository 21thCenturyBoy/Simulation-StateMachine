using System;

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

        public void SetState(IState state)
        {
            _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public virtual void Tick()
        {
            IState entry = _currentState?.GetFinalEntryState();
            if (entry != null)SetState(entry);
            _currentState?.Tick();
            IState to = _currentState?.GetTransitionState();
            if (to != null) SetState(to);
        }
    }
}
