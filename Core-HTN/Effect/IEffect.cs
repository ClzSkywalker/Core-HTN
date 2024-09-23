using CoreHTN.Context;

namespace CoreHTN.Effect;

public interface IEffect
{
    string Name { get; }
    EffectType Type { get; }
    void Apply(IContext ctx);
}