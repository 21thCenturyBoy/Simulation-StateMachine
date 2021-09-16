# StateMachine-Famework
适用于游戏或虚拟仿真(业务)的状态机框架。

为了避免一把梭if...else和switch式写逻辑。

## 特色

- 包含有限状态机FiniteStateMachine和层次状态机HierarchicalStateMachine的实现
- 可通过创建状态图及状态点，快速构建有限状态机
- 支持链式写法（Lambda）
- 状态可扩展，内置定时任务的状态、计时时间状态
- 提供状态改变事件
- SSMTool自动生成脚本工具**（暂时不可用）**
- 等...

## 代码示例

如下：

> ```c#
> private void InitFSM()
> {
> 	LampFsm = new FSM();
> 	//构造状态
> 	FiniteState idelState = new FiniteState("未通电状态");
> 	TimerFS brightState = new TimerFS("明亮状态", new TimeChunk(GetDeltaTime, 5000));
> 	TimerTaskFS flashState = new TimerTaskFS("闪烁状态", new TimeChunk(GetDeltaTime, 1000), () => { IsLight = !IsLight; }, 3);
> 	FiniteState blowoutState = new FiniteState("灯丝烧断状态");
> 	//状态关系
> 	idelState.To(brightState, () => IsPower).To(flashState, brightState.Finished).To(blowoutState, flashState.Finished);
> 	//设置初始状态
> 	LampFsm.SetState(idelState);
> 	//注册状态切换事件
> 	LampFsm.StateChangeEvent += LampFsm_StateChangeEvent; 
> }
> ```

更多示例请参考：

参考ExampleCode和[说明](Explain.md)

