using System;
using Humanizer;

namespace Fast.Shared.Infrastructure.Modules;

public static class To
{
    public static string RequestPath<TRequest>() where TRequest : class => typeof(TRequest).RequestPath();

    public static string RequestPath(this object request)
    {
        if (request is not Type type)
        {
            type = request.GetType();
        }

        return type.Name.Underscore();
    }
}