using CoreHTN;
using CoreHTN.Context;
using CoreHTN.Planner;
using CoreHTN.Task;
using CoreHTN.Task.CompoundTask;

namespace Core_HTN.UnitTests;

[TestClass]
public class SimpleTest
{
    enum MyWorldState
    {
    };

    private class MyContext : BaseContext<MyWorldState>
    {
        protected override List<MyWorldState> WorldState => [];
    }

    private static Domain<MyContext> BuildDomain()
    {
        var materialObject = new List<string>() { "item1", "item2" };
        var builder = new DomainBuilder<MyContext>("human");
        builder
            .CompoundTask<SequenceCompoundTask>("select item")
            .Condition("find idle user", (_) => materialObject.Count > 0)
            .Action("pick item")
            .Do((_) =>
            {
                materialObject.RemoveAt(0);
                Console.WriteLine("pick item");
                return TaskEnum.Success;
            })
            .End()
            .Action("deliver item")
            .Do((_) =>
            {
                Console.WriteLine("deliver item");
                return TaskEnum.Success;
            })
            .End()
            .End();

        builder.Select("walk")
            .Action("walk")
            .Do((_) =>
            {
                Console.WriteLine("walk:find item");
                materialObject.Add("item3");
                return TaskEnum.Success;
            })
            .End().End();

        return builder.Build();
    }

    [TestMethod]
    public void TestTick()
    {
        var domain = BuildDomain();
        var ctx = new MyContext();
        var planner = new Planner<MyContext>();
        ctx.Init();
        var count = 0;
        while (count < 10)
        {
            count++;
            planner.Tick(domain, ctx);
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