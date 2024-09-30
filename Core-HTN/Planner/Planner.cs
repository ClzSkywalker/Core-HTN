using CoreHTN.Context;
using CoreHTN.Task;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN.Planner;

public class Planner<T> where T : IContext
{
    public void Tick(Domain<T> domain, T ctx)
    {
        if (!ctx.IsInitialized)
        {
            throw new Exception("Context was not initialized!");
        }

        if (ShouldFindNewPlan(ctx))
        {
            domain.FindPlan(ctx, out var plan);
            ctx.PlannerState.Plan = plan;
            if (ctx.PlannerState.Plan.Count == 0)
            {
                return;
            }

            ctx.PlannerState.CurrentTask = plan.Dequeue();
            ctx.PlannerState.LastStatus = TaskEnum.Running;
        }

        // find next plan
        if (CanFindNextTask(ctx))
        {
            if (!SelectNextTask(ctx))
            {
                return;
            }
        }

        Execute(ctx);
    }

    private bool ShouldFindNewPlan(T ctx)
    {
        if (ctx.PlannerState.LastStatus == TaskEnum.Failure ||
            ctx.PlannerState.LastStatus != TaskEnum.Running && ctx.PlannerState.Plan.Count == 0)
        {
            ctx.ContextState = ContextState.Planning;
        }

        return ctx.IsDirty ||
               ctx.ContextState == ContextState.Planning;
    }

    private bool CanFindNextTask(T ctx)
    {
        return ctx.PlannerState is { LastStatus: TaskEnum.Success, Plan.Count: > 0 };
    }

    private bool SelectNextTask(T ctx)
    {
        ctx.PlannerState.CurrentTask = ctx.PlannerState.Plan.Dequeue();
        ctx.PlannerState.LastStatus = TaskEnum.Running;
        return ctx.PlannerState.CurrentTask.IsValid(ctx);
    }

    private void Execute(T ctx)
    {
        if (ctx.PlannerState.CurrentTask is not IPrimitiveTask task)
        {
            if (ctx.OpenLog)
            {
                Console.WriteLine($"Executing compound task: {ctx.PlannerState.CurrentTask}");
            }

            return;
        }

        if (task.Operator == null)
        {
            throw new Exception($"Task {task} has no operator!");
        }

        var status = task.Operator.Update(ctx);
        switch (status)
        {
            case TaskEnum.Failure:
                task.Operator.Aborted(ctx);
                ctx.ContextState = ContextState.Planning;
                break;
            case TaskEnum.Success:
                break;
            case TaskEnum.Running:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ctx.PlannerState.LastStatus = status;
    }
}