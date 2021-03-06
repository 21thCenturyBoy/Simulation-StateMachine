# StateMachine-Famework
适用于游戏或虚拟仿真(业务)的状态机框架。

## 特色

- 包含有限状态机和层次状态机的实现

- 可通过创建状态图及状态点，快速构建有限状态机
- 支持链式写法（Lambda）
- 状态可扩展，内置定时任务的状态、计时时间状态
- 等...

## 例子

#### 简单状态机

实现如下NPC角色的状态机。

![pic1](E:\Project\Git\Simulation-StateMachine\Simulation-StateMachine\diagrams\pic1.PNG)

NPC角色进入闲置状态5秒后，进入巡逻状态。在巡逻状态中受到伤害，进入逃跑状态。在逃跑5秒后，进入闲置状态。

```c#
        //构造状态机
        FSM npcFsm = new FSM();

        //创建时间块
        TimeChunk time5s = new TimeChunk(GetDeltatime, 5000);
		//状态机更新时间间隔单位为毫秒，GetDeltatime委托返回状态机距离上一次更新的时间差

        //创建状态
		//TimerFS为计时时间状态，通过Finished获取当前是否计时完毕
        TimerFS idle = new TimerFS("idle",time5s);
        TimerFS flee = new TimerFS("flee", time5s);
        FiniteState patrol = new FiniteState("patrol", PatrolonEnter, PatrolOnTick, PatrolonExit);

        idle.To(flee, idle.Finished).To(patrol, GetDanger);//GetDanger为委托
        
        npcFsm.SetState(idle);//设置初始状态
```
更新状态机。

```c#
      fsm.Tick();
```

------

#### 层次状态机

...

------

#### 使用状态图构建有限状态机

注意：当状态太多时，需考虑当前状态机的结构是否合理。网游一般不会超过10个，单机就不好说了。

实现一个仿真产品部件的状态机：

















未完待续...

More documentation coming soon...