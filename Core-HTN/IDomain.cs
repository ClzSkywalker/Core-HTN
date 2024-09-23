using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;

namespace CoreHTN;

public interface IDomain
{
    TaskRoot Root { get; }
    void Add(ICompoundTask parent, ITask subtask);
}