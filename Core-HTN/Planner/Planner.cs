using CoreHTN.Context;
using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;

namespace CoreHTN.Planner;

public class Planner<T> where T:IContext
{
    public void Tick(Domain<T> domain, T ctx)
    {
        if (!ctx.IsInitialized)
        {
            throw new Exception("Context was not initialized!");
        }
        
        var decompositionStatus = DecompositionEnum.Failed;
        var isTryingToReplacePlan = false;

        // 设定计划/找到当前计划/找到下一个计划
        
        // 执行计划/找到下一个计划
        
        
        if (ShouldFindNewPlan(ctx))
        {
            domain.FindPlan(ctx, out Queue<ITask> plan);
            ctx.PlannerState.Plan = plan;
            ctx.PlannerState.CurrentTask = plan.Dequeue();
            
        }
        
        
        
    }
    
    private bool ShouldFindNewPlan(T ctx)
    {
        return ctx.IsDirty || (ctx.PlannerState.CurrentTask == null && ctx.PlannerState.Plan.Count == 0);
    }
}