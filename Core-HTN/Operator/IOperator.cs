using CoreHTN.Context;
using CoreHTN.Task;

namespace CoreHTN.Operator;

public interface IOperator
{
    TaskEnum Update(IContext ctx);
    void Stop(IContext ctx);
    void Aborted(IContext ctx); 
}