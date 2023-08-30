using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Fast.Shared.Abstractions.Contracts;

public abstract class Contract<T> : IContract where T : class
{
    private readonly ISet<string> _properties = new HashSet<string>();
    
    public string Producer { get; }
    public Type Type { get; } = typeof(T);
    public IEnumerable<string> Properties => _properties;

    protected void Require(Expression<Func<T, object>> expression) => _properties.Add(GetName(expression));

    protected void RequireAll() => RequireAll(typeof(T));

    protected void Ignore(Expression<Func<T, object>> expression) => _properties.Remove(GetName(expression));

    protected void IgnoreAll() => _properties.Clear();

    protected Contract(string producer)
    {
        if (string.IsNullOrWhiteSpace(producer))
        {
            throw new InvalidOperationException("Contract type producer cannot be empty.");
        }
        
        Producer = producer;
    }

    private string GetName(Expression<Func<T, object>> expression)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            if (expression.Body is not UnaryExpression unaryExpression)
            {
                throw new InvalidOperationException("Invalid unary expression.");
            }

            memberExpression = unaryExpression.Operand as MemberExpression ?? throw new InvalidOperationException();
        }

        if (memberExpression is null)
        {
            throw new InvalidOperationException("Invalid member expression.");
        }

        var parts = expression.ToString().Split(",")[0].Split(".").Skip(1);
        var name = string.Join(".", parts);

        return name;
    }

    private void RequireAll(Type type, string? parent = null)
    {
        var originalContract = FormatterServices.GetUninitializedObject(type);
        var originalContractType = originalContract.GetType();
        foreach (var property in originalContractType.GetProperties())
        {
            var propertyName = string.IsNullOrWhiteSpace(parent) ? property.Name : $"{parent}.{property.Name}";
            _properties.Add(propertyName);
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                RequireAll(property.PropertyType, propertyName);
            }
        }
    }
}