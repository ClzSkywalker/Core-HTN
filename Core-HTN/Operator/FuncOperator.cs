using CoreHTN.Context;
using CoreHTN.Task;

namespace CoreHTN.Operator;

public class FuncOperator<T>(Func<T, TaskEnum> func, Action<T>? funcStop = null, Action<T>? funcAborted = null)
    : IOperator
    where T : IContext
{
    private readonly Func<T, TaskEnum> _func = func;
    private readonly Action<T>? _funcStop = funcStop;
    private readonly Action<T>? _funcAborted = funcAborted;

    // ========================================================= CONSTRUCTION

    public TaskEnum Update(IContext ctx)
    {
        if (ctx is T c)
        {
            return _func.Invoke(c);
        }

        throw new Exception("Unexpected context type!");
    }

    public void Stop(IContext ctx)
    {
        if (ctx is T c)
        {
            _funcStop?.Invoke(c);
        }
        else
        {
            throw new Exception("Unexpected context type!");
        }
    }

    public void Aborted(IContext ctx)
    {
        if (ctx is T c)
        {
            _funcAborted?.Invoke(c);
        }
        else
        {
            throw new Exception("Unexpected context type!");
        }
    }
}