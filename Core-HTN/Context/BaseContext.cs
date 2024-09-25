using CoreHTN.Condition;
using CoreHTN.Effect;
using CoreHTN.Planner;
using CoreHTN.Task;

namespace CoreHTN.Context;

public abstract class BaseContext<TState> : IContext
{
    public bool IsInitialized { get; private set; }
    public bool IsDirty { get; set; }
    public ContextState ContextState { get; set; } = ContextState.Executing;
    public virtual IPlannerState PlannerState { get; }
    public int CurrentDecompositionDepth { get; set; }
    public bool OpenLog { get; }

    public abstract List<TState> WorldState { get; }

    public bool HasState(TState state)
    {
        return WorldState.Contains(state);
    }

    public void SetState(TState state)
    {
        WorldState.Add(state);
    }


    public virtual void Log(string name, string description, int depth, ITask task,
        ConsoleColor color = ConsoleColor.White)
    {
        if (OpenLog == false)
        {
            return;
        }

        Console.WriteLine($"name: {name}, depth: {depth}, task: {task.GetType().Name}");
    }

    public virtual void Init()
    {
        IsInitialized = true;
    }

    public void Log(string name, string description, int depth, ICondition condition,
        ConsoleColor color = ConsoleColor.DarkGreen)
    {
        if (OpenLog == false)
        {
            return;
        }

        Console.WriteLine($"name: {name}, depth: {depth}, condition: {condition.GetType().Name}");
    }

    public void Log(string name, string description, int depth, IEffect effect,
        ConsoleColor color = ConsoleColor.DarkYellow)
    {
        if (OpenLog == false)
        {
            return;
        }

        Console.WriteLine($"name: {name}, depth: {depth}, effect: {effect.GetType().Name}");
    }
}