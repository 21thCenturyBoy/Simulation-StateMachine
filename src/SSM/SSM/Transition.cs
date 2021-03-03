using System;
using System.Collections.Generic;

namespace SSM
{
    public class Transition
    {
        public IState State { get; private set; }
        private List<Func<bool>> _tsConditions;

        public Transition(IState state) { State = state; }

        public Transition(IState state, params Func<bool>[] conditions)
        {
            State = state;
            _tsConditions = new List<Func<bool>>(conditions);
        }

        public bool CheckTransition()
        {
            bool check = true;
            if (_tsConditions == null) return check;
            foreach (Func<bool> c in _tsConditions)
            {
                if (!c())
                {
                    check = false;
                    break;
                }
            }
            return check;
        }
    }

}
