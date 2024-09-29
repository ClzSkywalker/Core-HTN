using CoreHTN;
using CoreHTN.Context;
using CoreHTN.Planner;
using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;

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

    private class MyContext : BaseContext<WorldState>
    {
        public override List<WorldState> WorldState { get; } = [];
        public override IPlannerState PlannerState { get; } = new DefaultPlannerState();

        public bool Done { get; set; }
    }

    private static Domain<MyContext> BuildDomain()
    {
        var user = new List<String>() { "user1", "user2" };
        var builder = new DomainBuilder<MyContext>("human");
        var task1 = builder
            .Select("deliver task")
            .Condition("find idle user", (ctx) => user.Count > 0)
            .CompoundTask<SequenceCompoundTask>("select item")
            .End().End();

        builder.Select("State2")
            .Condition("no a", ctx => !ctx.HasState(WorldState.State1))
            .Action("A2")
            .Do((ctx) =>
            {
                Console.WriteLine("A2");
                ctx.SetState(WorldState.State1);
                ctx.SetState(WorldState.State2);
                return TaskEnum.Success;
            })
            .End().End();

        builder.Select("Done")
            .Condition("down", (ctx) => ctx.HasState(WorldState.State3))
            .Action("Done")
            .Do((ctx) =>
            {
                Console.WriteLine("Done");
                ctx.Done = true;
                return TaskEnum.Success;
            })
            .End().End();
        return builder.Build();
    }

    [TestMethod]
    public void TestTick()
    {
        var domain = BuildDomain();
        var ctx1 = new MyContext();
        var planner = new Planner<MyContext>();
        ctx1.Init();
        var count = 0;
        while (!ctx1.Done && count < 4)
        {
            count++;
            planner.Tick(domain, ctx1);
        }
    }

    [TestMethod]
    public void TestFindPlan()
    {
        var ctx = new MyContext();
        var domain = BuildDomain();
        domain.FindPlan(ctx, out var plan);
        Console.WriteLine($"{plan.Count}");
    }
}