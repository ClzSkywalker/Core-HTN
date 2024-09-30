using CoreHTN.Context;

namespace CoreHTN.Task.CompoundTask;

public class SelectorCompoundTask : BaseCompoundTask
{
    protected override DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result)
    {
        Plan.Clear();
        var status = DecompositionEnum.Succeeded;
        for (var taskIndex = startIndex; taskIndex < SubsTasks.Count; taskIndex++)
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"Selector.OnDecompose:Task index: {taskIndex}: {SubsTasks[taskIndex].Name}");
            }

            var task = SubsTasks[taskIndex];

            status = OnDecomposeTask(ctx, task, taskIndex, out result);
            switch (status)
            {
                case DecompositionEnum.Succeeded:
                    goto BreakLoop;
                case DecompositionEnum.Rejected:
                case DecompositionEnum.Failed:
                default:
                    continue;
            }
        }

        BreakLoop:
        result = Plan;
        return status;
    }

    protected override DecompositionEnum OnDecomposeCompoundTask(IContext ctx,
        ICompoundTask task, int taskIndex,
        out Queue<ITask> result)
    {
        result = new Queue<ITask>();
        var status = task.Decompose(ctx, 0, out var subPlan);

        // If status is rejected, that means the entire planning procedure should cancel.
        switch (status)
        {
            case DecompositionEnum.Failed:
            case DecompositionEnum.Rejected:
                if (ctx.OpenLog)
                {
                    Log(ctx, $"Selector.OnDecomposeCompoundTask:{status}: Decomposing {task.Name} was rejected.",
                        ConsoleColor.Red);
                }

                result = new Queue<ITask>();
                break;
            case DecompositionEnum.Succeeded:
                while (subPlan.Count > 0)
                {
                    var p = subPlan.Dequeue();
                    if (ctx.OpenLog)
                    {
                        Log(ctx, $"Selector.OnDecomposeCompoundTask:Decomposing {task.Name}:Pushed {p.Name} to plan!",
                            ConsoleColor.Blue);
                    }

                    Plan.Enqueue(p);
                }

                result = Plan;
                break;
        }

        return status;
    }
}