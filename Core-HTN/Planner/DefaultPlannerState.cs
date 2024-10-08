using CoreHTN.Task;

namespace CoreHTN.Planner;

public class DefaultPlannerState:IPlannerState
{
    public ITask? CurrentTask { get; set; }
    public Queue<ITask> Plan { get; set; } = [];
    public TaskEnum LastStatus { get; set; }
}