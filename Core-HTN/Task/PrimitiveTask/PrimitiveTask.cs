using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Effect;
using CoreHTN.Operator;
using CoreHTN.Task.CompoundTask;

namespace CoreHTN.Task.PrimitiveTask;

public class PrimitiveTask : IPrimitiveTask
{
    public string Name { get; set; }
    public ICompoundTask Parent { get; set; }
    public List<ICondition> Conditions { get; set; } = [];
    public List<IEffect> Effects { get; } = [];

    public ITask AddCondition(ICondition condition)
    {
        Conditions.Add(condition);
        return this;
    }

    public ITask AddExecutingCondition(ICondition condition)
    {
        ExecutingConditions.Add(condition);
        return this;
    }


    public ITask AddEffect(IEffect effect)
    {
        Effects.Add(effect);
        return this;
    }

    public void ApplyEffects(IContext ctx)
    {
        if (ctx.ContextState == ContextState.Planning)
        {
            if (ctx.OpenLog)
            {
                Log(ctx, $"PrimitiveTask.ApplyEffects", ConsoleColor.Yellow);
            }
        }

        foreach (var effect in Effects)
        {
            effect.Apply(ctx);
        }
    }

    public bool IsValid(IContext ctx)
    {
        if (ctx.OpenLog)
        {
            Log(ctx, $"PrimitiveTask.IsValid check");
        }

        foreach (var condition in Conditions)
        {
            var result = condition.IsValid(ctx);
            if (ctx.OpenLog)
            {
                Log(ctx, $"PrimitiveTask.IsValid: {result}", result ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
            }

            if (!result)
            {
                return false;
            }
        }

        return true;
    }

    public DecompositionEnum OnIsValidFailed(IContext ctx)
    {
        throw new NotImplementedException();
    }

    public List<ICondition> ExecutingConditions { get; }


    public IOperator Operator { get; private set; }

    public void SetOperator(IOperator action)
    {
        if (Operator != null)
        {
            throw new InvalidOperationException("A Primitive Task can only contain a single Operator!");
        }

        Operator = action;
    }

    public void Stop(IContext ctx)
    {
        Operator.Stop(ctx);
    }

    public void Aborted(IContext ctx)
    {
        Operator.Aborted(ctx);
    }


    protected virtual void Log(IContext ctx, string description, ConsoleColor color = ConsoleColor.White)
    {
        ctx.Log(Name, description, ctx.CurrentDecompositionDepth + 1, this, color);
    }
}