using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Task.CompoundTask;

namespace CoreHTN.Task;

public interface ITask
{
    string Name { get; set; } 
    ICompoundTask? Parent { get; set; }
    List<ICondition> Conditions { get; set; }
    ITask AddCondition(ICondition condition);
    bool IsValid(IContext ctx);
    DecompositionEnum OnIsValidFailed(IContext ctx);
}