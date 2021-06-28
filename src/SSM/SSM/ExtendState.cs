using System;

namespace SSM
{
    /// <summary>
    /// 时间块
    /// </summary>
    public struct TimeChunk
    {
        public float Time;
        public Func<float> UpdateDleta;

        public TimeChunk(Func<float> updateDleta, float time)
        {
            UpdateDleta = updateDleta;
            Time = time;
        }
    }
    /// <summary>
    /// 计时周期任务状态
    /// </summary>
    public class TimerTaskFS : HierarchicalFS
    {
        private float _time = 0;
        private float _timer = 0;
        private int _count = 0;
        private int _counter = 0;
        private bool _loopflag = true;
        private Func<float> onUpdateTime;
        private Action _task;
        public float Timer => _timer;
        public TimerTaskFS(TimeChunk timeChunk, Action task) : this(task.Method.Name, timeChunk, task, null, null, null) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, int count) : this(task.Method.Name, timeChunk, task, null, null, null, count) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onTick) : this(task.Method.Name, timeChunk, task, null, onTick, null) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onTick, int count) : this(task.Method.Name, timeChunk, task, null, onTick, null, count) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onEnter, Action onExit) : this(task.Method.Name, timeChunk, task, onEnter, null, onExit) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onEnter, Action onExit, int count) : this(task.Method.Name, timeChunk, task, onEnter, null, onExit, count) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onEnter, Action onTick, Action onExit) : this(task.Method.Name, timeChunk, task, onEnter, onTick, onExit) { }
        public TimerTaskFS(TimeChunk timeChunk, Action task, Action onEnter, Action onTick, Action onExit, int count = 0) : this(task.Method.Name, timeChunk, task, onEnter, onTick, onExit, count) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task) : this(name, timeChunk, task, null, null, null) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, int count) : this(name, timeChunk, task, null, null, null, count) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onTick) : this(name, timeChunk, task, null, onTick, null) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onTick, int count) : this(name, timeChunk, task, null, onTick, null, count) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onEnter, Action onExit) : this(name, timeChunk, task, onEnter, null, onExit) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onEnter, Action onExit, int count) : this(name, timeChunk, task, onEnter, null, onExit, count) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onEnter, Action onTick, Action onExit) : this(name, timeChunk, task, onEnter, onTick, onExit, 0) { }
        public TimerTaskFS(string name, TimeChunk timeChunk, Action task, Action onEnter, Action onTick, Action onExit, int count = 0)
        {
            Name = name;
            _time = timeChunk.Time;
            onUpdateTime = timeChunk.UpdateDleta;
            _task = task;

            this.onEnter = Start;
            this.onEnter += onEnter;
            this.onTick = Update;
            this.onTick += onTick;
            this.onExit = End;
            this.onExit += onExit;

            _loopflag = count == 0;
            _count = count;
        }

        protected override void Start() { RestTimer(); RestCounter(); }
        protected override void Update()
        {
            if (_counter == 0 && !_loopflag) return;

            _timer -= onUpdateTime?.Invoke() ?? 0;
            if (_timer <= 0)
            {
                _task?.Invoke();
                RestTimer();
                _counter--;
            }
        }

        protected override void End() { }
        protected virtual void RestTimer() { _timer = _time; }
        protected virtual void RestCounter() { _counter = _count; }
        public int GetCounter() => _counter;
        public bool Finished() => _counter == 0 && !_loopflag;
    }
    /// <summary>
    /// 计时任务状态
    /// </summary>
    public class TimerFS : HierarchicalFS
    {
        private float _time = 0;
        private float _timer = 0;
        private Func<float> onUpdateTime;
        public float Timer => _timer;
        public TimerFS(string name, TimeChunk timeChunk) : this(name, timeChunk, null) { }
        public TimerFS(string name, TimeChunk timeChunk, Action onTick) : this(name, timeChunk, null, onTick, null) { }
        public TimerFS(string name, TimeChunk timeChunk, Action onEnter, Action onExit) : this(name, timeChunk, onEnter, null, onExit) { }
        public TimerFS(string name, TimeChunk timeChunk, Action onEnter, Action onTick, Action onExit)
        {
            _time = timeChunk.Time;
            Name = name;
            onUpdateTime = timeChunk.UpdateDleta;
            this.onEnter = Start;
            this.onEnter += onEnter;
            this.onTick = Update;
            this.onTick += onTick;
            this.onExit = End;
            this.onExit += onExit;
        }

        protected override void Start() { _timer = 0; }
        protected override void Update() { _timer += onUpdateTime?.Invoke() ?? 0; }
        protected override void End() { }
        public bool Finished() { return _timer >= _time; }

    }
}
