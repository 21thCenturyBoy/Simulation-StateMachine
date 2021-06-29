using System;

namespace SSM
{
    /// <summary>
    /// 状态机接口
    /// </summary>
    public interface IStateMachine
    {
        IState To(IState from, IState to);
        IState To(IState from, IState to, Func<bool> condition);

        void SetState(IState state);
        void Tick();
    }

    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FSM : IStateMachine
    {
        /// <summary>
        /// 状态改变事件
        /// </summary>
        public event Action<IState, IState> StateChangeEvent;

        protected IState _currentState;
        public IState CurrentState { get => _currentState; }
        public IState To(IState from, IState to) { from.To(to); return to; }

        public IState To(IState from, IState to, Func<bool> condition) { from.To(to, condition); return to; }

        public void SetState(IState state)
        {
            _currentState?.Exit();
            OnStateChangeEvent(_currentState, state);
            _currentState = state;
            _currentState.Enter();
        }

        public virtual void Tick()
        {
            IState entry = _currentState?.GetFinalEntryState();
            if (entry != null) SetState(entry);
            _currentState?.Tick();
            IState to = _currentState?.GetTransitionState();
            if (to != null) SetState(to);
        }

        protected virtual void OnStateChangeEvent(IState arg1, IState arg2)
        {
            StateChangeEvent?.Invoke(arg1, arg2);
        }
    }
}
