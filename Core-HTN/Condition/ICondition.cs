using CoreHTN.Context;

namespace CoreHTN.Condition;

public interface ICondition
{
    string Name { get; }
    bool IsValid(IContext ctx);
}