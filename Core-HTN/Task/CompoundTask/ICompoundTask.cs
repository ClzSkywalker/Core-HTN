using CoreHTN.Context;

namespace CoreHTN.Task.CompoundTask;

public interface ICompoundTask:ITask
{
    List<ITask> SubsTasks { get; }
    ICompoundTask AddSubtask(ITask task);
    
    /// <summary>
    /// The resolution takes the combined task as the root node
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="startIndex"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    DecompositionEnum Decompose(IContext ctx, int startIndex, out Queue<ITask> result);
}