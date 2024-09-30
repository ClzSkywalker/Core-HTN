using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN.Task.CompoundTask;

public abstract class BaseCompoundTask : ICompoundTask
{
    public string Name { get; set; } = "";
    public ICompoundTask? Parent { get; set; }
    public List<ICondition> Conditions { get; set; } = [];

    public List<ITask> SubsTasks { get; } = [];


    /// <summary>
    /// Planned task
    /// </summary>
    protected readonly Queue<ITask> Plan = new();
    /// <summary>
    /// 不同类型分解器入口 
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="startIndex"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected abstract DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result);
    
    /// <summary>
    /// 分解组合任务
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="task"></param>
    /// <param name="taskIndex"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected abstract DecompositionEnum OnDecomposeCompoundTask(IContext ctx, ICompoundTask task, int taskIndex,
        out Queue<ITask> result);

    /// <summary>
    /// Start method for task group decomposition
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="startIndex"></param>
    /// <param name="result"></param>
    /// <returns></returns>
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


    protected DecompositionEnum OnDecomposePrimitiveTask(IContext ctx,
        IPrimitiveTask task, out Queue<ITask> result)
    {
        if (!task.IsValid(ctx))
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"OnDecomposePrimitiveTask:Failed:Task {task.Name}.IsValid returned false!", ConsoleColor.Red);
            }

            result = Plan;
            return task.OnIsValidFailed(ctx);
        }

        if (ctx.OpenLog)
        {
            Log(ctx, $"OnDecomposeTask:Pushed {task.Name} to plan!", ConsoleColor.Blue);
        }

        task.ApplyEffects(ctx);
        Plan.Enqueue(task);
        result = Plan;
        return DecompositionEnum.Succeeded;
    }

    protected virtual void Log(IContext ctx, string description, ConsoleColor color = ConsoleColor.White)
    {
        ctx.Log(Name, description, ctx.CurrentDecompositionDepth, this, color);
    }



    /// <summary>
    /// 分解任务
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="task"></param>
    /// <param name="taskIndex"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected DecompositionEnum OnDecomposeTask(IContext ctx, ITask task, int taskIndex,
        out Queue<ITask> result)
    {
        if (!task.IsValid(ctx))
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"Selector.OnDecomposeTask:Failed:Task {task.Name}.IsValid returned false!", ConsoleColor.Red);
            }

            result = Plan;
            return task.OnIsValidFailed(ctx);
        }

        DecompositionEnum status;
        switch (task)
        {
            case ICompoundTask value:
                return OnDecomposeCompoundTask(ctx, value, taskIndex, out result);

            case IPrimitiveTask value:
                status = OnDecomposePrimitiveTask(ctx, value, out result);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        result = Plan;
        if (ctx.OpenLog)
        {
            Log(ctx, $"OnDecomposeTask:Task {task.Name}: {status}");
        }

        return status;
    }
}