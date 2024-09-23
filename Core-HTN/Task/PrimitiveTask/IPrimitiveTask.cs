using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Effect;
using CoreHTN.Operator;

namespace CoreHTN.Task.PrimitiveTask;

public interface IPrimitiveTask:ITask
{
    /// <summary>
    ///     Executing conditions are validated before every call to Operator.Update(...)
    /// </summary>
    List<ICondition> ExecutingConditions { get; }
    /// <summary>
    ///     Add a new executing condition to the primitive task. This will be checked before
    ///		every call to Operator.Update(...)
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    ITask AddExecutingCondition(ICondition condition);
    IOperator Operator { get; }
    void SetOperator(IOperator action);
    void Stop(IContext ctx);
    void Aborted(IContext ctx);
    
    List<IEffect> Effects { get; }
    ITask AddEffect(IEffect effect);
    void ApplyEffects(IContext ctx);
}