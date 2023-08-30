using System.Threading.Tasks;

namespace Fast.Shared.Infrastructure.Initializers;

public interface IInitializer
{
    Task InitAsync();
}