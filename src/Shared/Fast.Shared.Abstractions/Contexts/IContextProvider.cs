namespace Fast.Shared.Abstractions.Contexts;

public interface IContextProvider
{
    IContext Current();
}