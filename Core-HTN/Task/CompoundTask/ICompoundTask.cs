using CoreHTN.Context;

namespace CoreHTN.Task.CompoundTask;

public interface ICompoundTask:ITask
{
    List<ITask> SubsTasks { get; }
    ICompoundTask AddSubtask(ITask task);
    DecompositionEnum Decompose(IContext ctx, int startIndex, out Queue<ITask> result);
}