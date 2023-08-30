using System.Threading;
using Fast.Shared.Abstractions.Contexts;

namespace Fast.Shared.Infrastructure.Contexts;

internal sealed class ContextAccessor
{
    private static readonly AsyncLocal<ContextHolder> Holder = new();

    public IContext? Context
    {
        get => Holder.Value?.Context;
        set
        {
            var holder = Holder.Value;
            if (holder is not null)
            {
                holder.Context = null;
            }

            if (value is not null)
            {
                Holder.Value = new ContextHolder {Context = value};
            }
        }
    }

    private class ContextHolder
    {
        public IContext? Context;
    }
}