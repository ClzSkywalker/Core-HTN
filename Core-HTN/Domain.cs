using System.Numerics;
using CoreHTN.Context;
using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;

namespace CoreHTN;

public class Domain<T>(string name) : IDomain
    where T : IContext
{
    public TaskRoot Root { get; } = new() { Name = name };

    public void Add(ICompoundTask parent, ITask subtask)
    {
        if (parent == subtask)
        {
            throw new Exception("Parent-task and Sub-task can't be the same instance!");
        }

        parent.AddSubtask(subtask);
        subtask.Parent = parent;
    }

    public DecompositionEnum FindPlan(T ctx, out Queue<ITask> plan )
    {
        var status=Root.Decompose(ctx, 0,out plan);
        ctx.ContextState = ContextState.Executing;
        return status;
    }
}