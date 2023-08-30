using System;
using System.Threading.Tasks;

namespace Fast.Shared.Infrastructure.Postgres;

public interface IUnitOfWork
{
    Task ExecuteAsync(Func<Task> action);
}