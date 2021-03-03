using System;
using System.Collections.Generic;

namespace SSM
{
    public interface IState
    {
        string Name { get; set; }
        void Enter();
        void Tick();
        void Exit();

        IState To(IState to);
        IState To(IState to, params Func<bool>[] conditions);

        //IState GetFinalEntryState();
        IState GetTransitionState();

    }


    public class FiniteState : IState
    {
        protected Action onEnter;
        protected Action onTick;
        protected Action onExit;
        private List<Transition> _transitions = new List<Transition>();
        public string Name { get; set; }

        public FiniteState() { onEnter = Start; onTick = Update; onExit = End; Name = "FiniteState"; }
        public FiniteState(string name) : this() { Name = name; }
        public FiniteState(string name, Action onTick) : this(name) { this.onTick = onTick; }
        public FiniteState(string name, Action onEnter, Action onTick) : this(name, onTick) { this.onEnter = onEnter; }
        public FiniteState(string name, Action onEnter, Action onTick, Action onExit) : this(name, onEnter, onTick) { this.onExit = onExit; }
        public FiniteState(Action onTick) : this(onTick.Method.Name) { this.onTick = onTick; }
        public FiniteState(Action onEnter, Action onTick) : this(onTick) { this.onEnter = onEnter; }
        public FiniteState(Action onEnter, Action onTick, Action onExit) : this(onEnter, onTick) { this.onExit = onExit; }

        public void Enter() { onEnter?.Invoke(); }
        public void Tick() { onTick?.Invoke(); }
        public void Exit() { onExit?.Invoke(); }

        public IState To(IState to) => To(to, null);

        public IState To(IState to, params Func<bool>[] conditions)
        {
            _transitions.Add(conditions == null ? new Transition(to) : new Transition(to, conditions));
            return to;
        }
        public IState GetTransitionState()
        {
            foreach (Transition t in _transitions) if (t.CheckTransition()) return t.State;
            return null;
        }

        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void End() { }
    }

    public class HierarchicalFiniteState : FiniteState
    {
        protected List<HierarchicalFiniteState> parents = new List<HierarchicalFiniteState>();
        protected HierarchicalFiniteState _entryState;
        public void AddChildren(params HierarchicalFiniteState[] children)
        {
            _entryState = children[0];
            foreach (HierarchicalFiniteState child in children) child.AddParent(this);
        }
        public void AddParent(HierarchicalFiniteState parent) { parents.Add(parent); }
        public void SetEntryState(HierarchicalFiniteState hfsmState) { _entryState = hfsmState; }

        public HierarchicalFiniteState GetFinalEntryState()
        {
            if (_entryState == null || _entryState._entryState == null) return _entryState;
            return _entryState.GetFinalEntryState();
        }
    }


}
