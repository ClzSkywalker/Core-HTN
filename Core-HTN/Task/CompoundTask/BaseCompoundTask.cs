using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN.Task.CompoundTask;

public abstract class BaseCompoundTask : ICompoundTask
{
    public string Name { get; set; }
    public ICompoundTask Parent { get; set; }
    public List<ICondition> Conditions { get; set; } = [];

    public List<ITask> SubsTasks { get; } = [];


    public DecompositionEnum Decompose(IContext ctx, int startIndex, out Queue<ITask> result)
    {
        if (ctx.OpenLog)
        {
            ctx.CurrentDecompositionDepth++;
        }

        var status = OnDecompose(ctx, startIndex, out result);

        if (ctx.OpenLog)
        {
            ctx.CurrentDecompositionDepth--;
        }

        return status;
    }

    public ITask AddCondition(ICondition condition)
    {
        Conditions.Add(condition);
        return this;
    }

    public virtual bool IsValid(IContext ctx)
    {
        foreach (var condition in Conditions)
        {
            var result = condition.IsValid(ctx);

            if (ctx.OpenLog)
            {
                Log(ctx,
                    $"CompoundTask.IsValid:{(result ? "Success" : "Failed")}:{condition.Name} is{(result ? "" : " not")} valid!",
                    result ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
            }

            if (result == false)
            {
                return false;
            }
        }

        return true;
    }

    public DecompositionEnum OnIsValidFailed(IContext ctx)
    {
        return DecompositionEnum.Failed;
    }

    public ICompoundTask AddSubtask(ITask subtask)
    {
        SubsTasks.Add(subtask);
        return this;
    }

    protected virtual void Log(IContext ctx, string description, ConsoleColor color = ConsoleColor.White)
    {
        ctx.Log(Name, description, ctx.CurrentDecompositionDepth, this, color);
    }

    protected abstract DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result);

    protected abstract DecompositionEnum OnDecomposeTask(IContext ctx, ITask task, int taskIndex,
        out Queue<ITask> result);

    protected abstract void OnDecomposePrimitiveTask(IContext ctx, IPrimitiveTask task, int taskIndex,
        out Queue<ITask> result);

    protected abstract DecompositionEnum OnDecomposeCompoundTask(IContext ctx, ICompoundTask task, int taskIndex,
        out Queue<ITask> result);
}