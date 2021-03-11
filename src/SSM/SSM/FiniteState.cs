using System;
using System.Collections.Generic;

namespace SSM
{
    public class FiniteStateException : Exception
    {
        public enum FiniteStateExceptionType
        {
            FiniteStateParentAlreadyExist = 10001,
            AddChildrenFail = 10002
        }

        public FiniteStateExceptionType ExceptionType { get; private set; }

        public FiniteStateException(FiniteStateExceptionType type) : this(type.ToString()) { }

        public FiniteStateException(string message) : base(message) { }

        public FiniteStateException(string message, Exception innerException) : base(message, innerException) { }

    }

    public interface IState
    {
        string Name { get; set; }
        void Enter();
        void Tick();
        void Exit();

        IState To(IState to);
        IState To(IState to, params Func<bool>[] conditions);
        IState OrTo(IState to, params Func<bool>[] conditions);
        IState GetTransitionState();
        IState GetFinalEntryState();
    }


    public class FiniteState : IState, IEquatable<FiniteState>
    {
        protected Action onEnter;
        protected Action onTick;
        protected Action onExit;
        protected List<Transition> transitions = new List<Transition>();
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
            transitions.Add(conditions == null ? new Transition(to) : new Transition(to, conditions));
            return to;
        }


        public IState OrTo(IState to, params Func<bool>[] conditions)
        {
            transitions.Add(conditions == null ? new Transition(to) : new Transition(to, conditions));
            return this;
        }

        public virtual IState GetTransitionState()
        {
            foreach (Transition t in transitions) if (t.CheckTransition()) return t.State;
            return null;
        }

        public virtual IState GetFinalEntryState() => null;

        public bool Equals(FiniteState other)
        {
            return string.Equals(Name, other.Name);
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return Equals((FiniteState)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void End() { }

    }

    public class HierarchicalFS : FiniteState
    {
        //Save Parents保存父节点
        protected HierarchicalFS parent;
        protected HierarchicalFS entryState;
        public void AddChildren(params HierarchicalFS[] children)
        {
            //if (children == null) throw new FiniteStateException(FiniteStateException.FiniteStateExceptionType.AddChildrenFail);

             entryState = children[0];
            foreach (HierarchicalFS child in children) child.AddParent(this);
        }
        public void AddParent(HierarchicalFS parentNoed) { parent = parentNoed; }
        //设置进入节点
        public void SetEntryState(HierarchicalFS hfsmState) { entryState = hfsmState; }

        public HierarchicalFS() { onEnter = Start; onTick = Update; onExit = End; Name = "HierarchicalFS"; }
        public HierarchicalFS(string name) { Name = name; }
        public HierarchicalFS(string name, Action onTick) : this(name) { this.onTick = onTick; }
        public HierarchicalFS(string name, Action onEnter, Action onTick) : this(name, onTick) { this.onEnter = onEnter; }
        public HierarchicalFS(string name, Action onEnter, Action onTick, Action onExit) : this(name, onEnter, onTick) { this.onExit = onExit; }
        public HierarchicalFS(Action onTick) : this(onTick.Method.Name) { this.onTick = onTick; }
        public HierarchicalFS(Action onEnter, Action onTick) : this(onTick) { this.onEnter = onEnter; }
        public HierarchicalFS(Action onEnter, Action onTick, Action onExit) : this(onEnter, onTick) { this.onExit = onExit; }
        public HierarchicalFS(string name, params HierarchicalFS[] children) { Name = name; AddChildren(children); }
        public HierarchicalFS(string name, Action onTick, params HierarchicalFS[] children) : this(name, children) { this.onTick = onTick; }
        public HierarchicalFS(string name, Action onEnter, Action onTick,params HierarchicalFS[] children) : this(name, onTick,children) { this.onEnter = onEnter; }
        public HierarchicalFS(string name, Action onEnter, Action onTick, Action onExit, params HierarchicalFS[] children) : this(name, onEnter, onTick,children) { this.onExit = onExit; }
        public HierarchicalFS(params HierarchicalFS[] children) : this(children[0].Name + "*") { AddChildren(children); }
        public HierarchicalFS(Action onTick, params HierarchicalFS[] children) : this(onTick.Method.Name,children) { this.onTick = onTick; }
        public HierarchicalFS(Action onEnter, Action onTick, params HierarchicalFS[] children) : this(onTick,children) { this.onEnter = onEnter; }
        public HierarchicalFS(Action onEnter, Action onTick, Action onExit, params HierarchicalFS[] children) : this(onEnter, onTick,children) { this.onExit = onExit; }


        public override IState GetTransitionState()
        {
            IState s = parent?.GetTransitionState();
            if (s != null) return s;
            return base.GetTransitionState();
        }

        public override IState GetFinalEntryState()
        {
            if (entryState == null || entryState.entryState == null) return entryState;
            return entryState.GetFinalEntryState();
        }
    }


}
