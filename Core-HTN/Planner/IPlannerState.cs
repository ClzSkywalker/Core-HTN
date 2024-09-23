using CoreHTN.Task;

namespace CoreHTN.Planner;

public interface IPlannerState
{
    ITask? CurrentTask { get; set; }
    Queue<ITask> Plan { get; set; }
    TaskStatus LastStatus { get; set; } 
}