using CoreHTN.Context;

namespace CoreHTN.Condition;

public class FuncCondition<T>(string name, Func<T, bool> func) : ICondition
    where T : IContext
{
    public readonly Func<T, bool> _func = func;
    public string Name { get; } = name;

    public bool IsValid(IContext ctx)
    {
        if (ctx is not T ctxT)
        {
            throw new Exception($"Context is not of type {typeof(T).Name}");
        }

        var result = _func.Invoke(ctxT);
        if (ctx.OpenLog)
        {
            ctx.Log(Name, $"FuncCondition.IsValid:{result}", ctx.CurrentDecompositionDepth+1, this, result ? ConsoleColor.DarkGreen : ConsoleColor.DarkRed);
        }

        return result;
    }
}