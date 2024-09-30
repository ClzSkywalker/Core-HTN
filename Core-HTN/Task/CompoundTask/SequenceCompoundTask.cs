using CoreHTN.Context;

namespace CoreHTN.Task.CompoundTask;

public class SequenceCompoundTask : BaseCompoundTask
{
    protected override DecompositionEnum OnDecompose(IContext ctx, int startIndex, out Queue<ITask> result)
    {
        Plan.Clear();
        DecompositionEnum status = DecompositionEnum.Succeeded;
        for (var taskIndex = startIndex; taskIndex < SubsTasks.Count; taskIndex++)
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"Sequence.OnDecompose:Task index: {taskIndex}: {SubsTasks[taskIndex].Name}");
            }

            var task = SubsTasks[taskIndex];

            status = OnDecomposeTask(ctx, task, taskIndex, out result);
            switch (status)
            {
                case DecompositionEnum.Succeeded:
                    continue;
                case DecompositionEnum.Rejected:
                case DecompositionEnum.Failed:
                    Plan.Clear();
                    goto BreakLoop;
            }
        }

        BreakLoop:
        result = Plan;
        return status;
    }

    protected override DecompositionEnum OnDecomposeCompoundTask(IContext ctx, ICompoundTask task, int taskIndex,
        out Queue<ITask> result)
    {
        result = [];
        var status = task.Decompose(ctx, 0, out var subPlan);
        switch (status)
        {
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
            case DecompositionEnum.Succeeded:
                while (subPlan.Count > 0)
                {
                    var p = subPlan.Dequeue();
                    if (ctx.OpenLog)
                    {
                        Log(ctx, $"Sequence.OnDecomposeCompoundTask:Adding {task.Name} push task: {p.Name}");
                    }

                    Plan.Enqueue(p);
                }

                result = Plan;
                break;
        }

        return status;
    }
}