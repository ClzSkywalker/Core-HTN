using System.Collections;
using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN.Task.CompoundTask;

public class SequenceCompoundTask : BaseCompoundTask
{
    protected readonly Queue<ITask> Plan = new();

    protected override DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result)
    {
        Plan.Clear();
        for (var taskIndex = startIndex; taskIndex < SubsTasks.Count; taskIndex++)
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"Sequence.OnDecompose:Task index: {taskIndex}: {SubsTasks[taskIndex].Name}");
            }

            var task = SubsTasks[taskIndex];

            var status = OnDecomposeTask(ctx, task, taskIndex, out result);
            switch (status)
            {
                case DecompositionEnum.Succeeded:
                    continue;
                case DecompositionEnum.Rejected:
                case DecompositionEnum.Failed:
                default:
                    break;
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
            Plan.Clear();
            result = Plan;
            return task.OnIsValidFailed(ctx);
        }

        switch (task.GetType())
        {
            case ICompoundTask value:
                return OnDecomposeCompoundTask(ctx, value, taskIndex, out result);
            case IPrimitiveTask value:
                OnDecomposePrimitiveTask(ctx, value, taskIndex, out result);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        result = Plan;
        var res = result.Count == 0 ? DecompositionEnum.Failed : DecompositionEnum.Succeeded;
        if (ctx.OpenLog)
        {
            Log(ctx, $"Sequence.OnDecomposeTask:Task {task.Name}: {res}");
        }

        return res;
    }

    protected override void OnDecomposePrimitiveTask(IContext ctx, IPrimitiveTask task, int taskIndex,
        out Queue<ITask> result)
    {
        if (ctx.OpenLog)
        {
            Log(ctx, $"Sequence.OnDecomposeTask:Pushed {task.Name} to plan!", ConsoleColor.Blue);
        }

        task.ApplyEffects(ctx);
        Plan.Enqueue(task);
        result = Plan;
    }

    protected override DecompositionEnum OnDecomposeCompoundTask(IContext ctx, ICompoundTask task, int taskIndex,
        out Queue<ITask> result)
    {
        var status = task.Decompose(ctx, 0, out var subPlan);
        switch (status)
        {
            case DecompositionEnum.Succeeded:
                Plan.Enqueue(task);
                result = Plan;
                break;
            case DecompositionEnum.Rejected:
            case DecompositionEnum.Failed:
                if (ctx.OpenLog)
                {
                    Log(ctx, $"Sequence.OnDecomposeCompoundTask:Failed:Task {task.Name}.Decompose returned {status}!",
                        ConsoleColor.Red);
                }

                Plan.Clear();
                result = Plan;
                break;
            default:
                throw new IndexOutOfRangeException();
        }

        return status;
    }
}