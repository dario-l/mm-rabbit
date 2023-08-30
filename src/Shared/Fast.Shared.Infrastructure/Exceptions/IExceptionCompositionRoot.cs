using System;
using Fast.Shared.Abstractions.Exceptions;

namespace Fast.Shared.Infrastructure.Exceptions;

internal interface IExceptionCompositionRoot
{
    ExceptionResponse? Map(Exception exception);
}