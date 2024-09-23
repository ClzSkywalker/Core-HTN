using CoreHTN.Condition;
using CoreHTN.Effect;
using CoreHTN.Planner;
using CoreHTN.Task;

namespace CoreHTN.Context;

public enum ContextState
{
    Planning,
    Executing
}

public interface IContext
{
    bool IsInitialized { get; }
    bool IsDirty { get; set; }
    ContextState ContextState { get; set; } 
    IPlannerState PlannerState { get; }
    int CurrentDecompositionDepth { get; set; }
    // 
    bool LogDecomposition { get; }
    void Log(string name, string description, int depth, ITask task, ConsoleColor color = ConsoleColor.White);
    void Log(string name, string description, int depth, ICondition condition, ConsoleColor color = ConsoleColor.DarkGreen);
    void Log(string name, string description, int depth, IEffect effect, ConsoleColor color = ConsoleColor.DarkYellow);
}