using CoreHTN;
using CoreHTN.Context;
using CoreHTN.Planner;
using CoreHTN.Task;

namespace Core_HTN.UnitTests;

[TestClass]
public class SimpleTest
{
    enum WorldState
    {
        State1,
        State2,
        State3,
        State4,
        State5
    };

    class MyContext : BaseContext<WorldState>
    {
        public override List<WorldState> WorldState { get; } = [];
        public override IPlannerState PlannerState { get; } = new DefaultPlannerState();

        public bool Done { get; set; }
    }

    [TestMethod]
    public void TestMethod1()
    {
        var domain = new DomainBuilder<MyContext>("human")
            .Select("State1")
            .Condition("has 1 and 2", (ctx) => ctx.HasState(WorldState.State1) && ctx.HasState(WorldState.State2))
            .Action("A1")
            .Do((ctx) =>
            {
                Console.WriteLine("A1");
                ctx.HasState(WorldState.State3);
                return TaskEnum.Success;
            })
            .End()
            .End()
            .Select("State2")
            .Condition("no a", ctx => !ctx.HasState(WorldState.State1))
            .Action("A2")
            .Do((ctx) =>
            {
                Console.WriteLine("A2");
                ctx.SetState(WorldState.State1);
                ctx.HasState(WorldState.State2);
                return TaskEnum.Success;
            })
            .End()
            .End()
            .Select("Done")
            .Condition("down", (ctx) => ctx.HasState(WorldState.State3))
            .Action("Done")
            .Do((ctx) =>
            {
                Console.WriteLine("Done");
                ctx.Done = true;
                return TaskEnum.Success;
            })
            .End().End()
            .Build();
        var ctx1 = new MyContext();
        var planner = new Planner<MyContext>();
        ctx1.Init();
        int count = 0;
        while (!ctx1.Done && count < 4)
        {
            count++;
            planner.Tick(domain, ctx1);
        }
    }
}