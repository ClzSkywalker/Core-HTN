using CoreHTN.Condition;
using CoreHTN.Context;
using CoreHTN.Effect;
using CoreHTN.Operator;
using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;
using CoreHTN.Task.PrimitiveTask;

namespace CoreHTN;

public abstract class BaseDomainBuilder<TBase, T>
    where TBase : BaseDomainBuilder<TBase, T>
    where T : IContext
{
    protected readonly Domain<T> Domain;
    protected readonly List<ITask> Pointers = [];

    protected BaseDomainBuilder(string name)
    {
        Domain = new Domain<T>(name);
        Pointers.Add(Domain.Root);
    }

    public ITask? Pointer
    {
        get
        {
            var count = Pointers.Count;
            return count == 0 ? null : Pointers[count-1];
        }
    }


    public TBase Select(string name)
    {
        return CompoundTask<SelectorCompoundTask>(name);
    }

    public TBase CompoundTask<TP>(string name) where TP : ICompoundTask, new()
    {
        var parent = new TP();
        return CompoundTask(name, parent);
    }

    public TBase CompoundTask<TP>(string name, TP task) where TP : ICompoundTask
    {
        if (Pointer is ICompoundTask compoundTask)
        {
            task.Name = name;
            Domain.Add(compoundTask, task);
            Pointers.Add(task);
        }
        else
        {
            throw new Exception(
                "Pointer is not a compound task type. Did you forget an End() after a Primitive Task Action was defined?");
        }

        return (TBase)this;
    }

    public TBase Action(string name)
    {
        return PrimitiveTask<PrimitiveTask>(name);
    }

    public TBase PrimitiveTask<TP>(string name) where TP : IPrimitiveTask, new()
    {
        if (Pointer is ICompoundTask compoundTask)
        {
            var parent = new TP { Name = name };
            Domain.Add(compoundTask, parent);
            Pointers.Add(parent);
        }
        else
        {
            throw new Exception(
                "Pointer is not a compound task type. Did you forget an End() after a Primitive Task Action was defined?");
        }

        return (TBase)this;
    }

    public TBase Do(Func<T, TaskEnum> action, Action<T>? forceStopAction = null)
    {
        if (Pointer is not IPrimitiveTask task)
        {
            throw new InvalidOperationException("Tried to add an Operator, but the Pointer is not a Primitive Task!");
        }

        var op = new FuncOperator<T>(action, forceStopAction);
        task.SetOperator(op);
        return (TBase)this;
    }

    public TBase Action(string name, Func<T, TaskEnum> action, Action<T>? forceStopAction = null)
    {
        Action(name);
        Do(action, forceStopAction);
        return (TBase)this;
    }

    public TBase Condition(string name, Func<T, bool> condition)
    {
        var cond = new FuncCondition<T>(name, condition);
        Pointer?.AddCondition(cond);
        return (TBase)this;
    }

    public TBase Effect(string name, EffectType effectType, Action<T, EffectType> action)
    {
        if (Pointer is not IPrimitiveTask task)
        {
            throw new Exception("Tried to add an Effect, but the Pointer is not a Primitive Task!");
        }
        var effect=new ActionEffect<T>(name, effectType, action);
        task.AddEffect(effect);
        return (TBase)this;
    }

    public TBase End()
    {
        Pointers.RemoveAt(Pointers.Count-1);
        return (TBase)this;
    }

    public Domain<T> Build()
    {
        if (Pointer != Domain.Root)
        {
            throw new Exception($"The domain definition lacks one or more End() statements. Pointer is '{Pointer?.Name}', but expected '{Domain.Root.Name}'.");
        }

        Pointers.Clear();
        return Domain;
    }
}