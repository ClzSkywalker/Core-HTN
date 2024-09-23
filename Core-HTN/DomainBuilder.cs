using CoreHTN.Context;

namespace CoreHTN;

public sealed class DomainBuilder<T>:BaseDomainBuilder<DomainBuilder<T>,T> where T:IContext
{
    public DomainBuilder(string name) : base(name)
    {
    }
}