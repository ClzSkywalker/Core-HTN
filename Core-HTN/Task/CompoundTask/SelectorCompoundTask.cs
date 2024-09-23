using CoreHTN.Context;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN.Task.CompoundTask;

public class SelectorCompoundTask : BaseCompoundTask
{
    protected readonly Queue<ITask> Plan = new();

    protected override DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result)
    {
        Plan.Clear();
        for (var taskIndex = startIndex; taskIndex < SubsTasks.Count; taskIndex++)
        {
            if (ctx.LogDecomposition)
            {
                Log(ctx, $"Selector.OnDecompose:Task index: {taskIndex}: {SubsTasks[taskIndex].Name}");
            }

            var task = SubsTasks[taskIndex];

            var status = OnDecomposeTask(ctx, task, taskIndex, out result);
            switch (status)
            {
                case DecompositionEnum.Succeeded:
                    return status;
                case DecompositionEnum.Rejected:
                case DecompositionEnum.Failed:
                default:
                    continue;
            }
        }

        result = Plan;
        return result.Count == 0 ? DecompositionEnum.Failed : DecompositionEnum.Succeeded;
    }

    protected override DecompositionEnum OnDecomposeTask(IContext ctx, ITask task, int taskIndex,
        out Queue<ITask> result)
    {
        if (!task.IsValid(ctx))
        {
            if (ctx.LogDecomposition)
            {
                Log(ctx, $"Selector.OnDecomposeTask:Failed:Task {task.Name}.IsValid returned false!", ConsoleColor.Red);
            }

            result = Plan;
            return task.OnIsValidFailed(ctx);
        }

        if (task is ICompoundTask compoundTask)
        {
            return OnDecomposeCompoundTask(ctx, compoundTask, taskIndex, out result);
        }

        if (task is IPrimitiveTask primitiveTask)
        {
            OnDecomposePrimitiveTask(ctx, primitiveTask, taskIndex, out result);
        }

        result = Plan;
        var status = result.Count == 0 ? DecompositionEnum.Failed : DecompositionEnum.Succeeded;
        if (ctx.LogDecomposition)
        {
            Log(ctx, $"Selector.OnDecomposeTask:{status}!",
                status == DecompositionEnum.Succeeded ? ConsoleColor.Green : ConsoleColor.Red);
        }

        return status;
    }

    protected override void OnDecomposePrimitiveTask(IContext ctx, IPrimitiveTask task, int taskIndex,
        out Queue<ITask> result)
    {
        if (ctx.LogDecomposition)
        {
            Log(ctx, $"Selector.OnDecomposeTask:Pushed {task.Name} to plan!", ConsoleColor.Blue);
        }

        task.ApplyEffects(ctx);
        Plan.Enqueue(task);
        result = Plan;
    }

    protected override DecompositionEnum OnDecomposeCompoundTask(IContext ctx,
        ICompoundTask task, int taskIndex,
        out Queue<ITask> result)
    {
        var status = task.Decompose(ctx, 0, out var subPlan);

        // If status is rejected, that means the entire planning procedure should cancel.
        if (status == DecompositionEnum.Rejected ||
            status == DecompositionEnum.Failed)
        {
            if (ctx.LogDecomposition)
            {
                Log(ctx, $"Selector.OnDecomposeCompoundTask:{status}: Decomposing {task.Name} was rejected.",
                    ConsoleColor.Red);
            }

            result = new Queue<ITask>();
            return status;
        }

        while (subPlan.Count > 0)
        {
            var p = subPlan.Dequeue();
            if (ctx.LogDecomposition)
            {
                Log(ctx, $"Selector.OnDecomposeCompoundTask:Decomposing {task.Name}:Pushed {p.Name} to plan!",
                    ConsoleColor.Blue);
            }

            Plan.Enqueue(p);
        }

        result = Plan;
        var s = result.Count == 0 ? DecompositionEnum.Failed : DecompositionEnum.Succeeded;

        if (ctx.LogDecomposition)
        {
            Log(ctx, $"Selector.OnDecomposeCompoundTask:{s}!",
                s == DecompositionEnum.Succeeded ? ConsoleColor.Green : ConsoleColor.Red);
        }

        return s;
    }
}