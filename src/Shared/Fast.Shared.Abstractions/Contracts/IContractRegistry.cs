namespace Fast.Shared.Abstractions.Contracts;

public interface IContractRegistry
{
   IContractRegistry Register<T>() where T : class, IContract;

   IContractRegistry RegisterPath(string path);

   IContractRegistry RegisterPath<TRequest, TResponse>(string path)
      where TRequest : class, IContract where TResponse : class, IContract;

   IContractRegistry RegisterPathWithRequest<TRequest>(string path)
      where TRequest : class, IContract;

   IContractRegistry RegisterPathWithResponse<TResponse>(string path)
      where TResponse : class, IContract;
}