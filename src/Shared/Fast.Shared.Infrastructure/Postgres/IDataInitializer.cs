using System.Threading.Tasks;

namespace Fast.Shared.Infrastructure.Postgres;

public interface IDataInitializer
{
    Task InitAsync();
}