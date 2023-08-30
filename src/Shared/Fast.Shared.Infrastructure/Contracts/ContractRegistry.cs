using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Fast.Shared.Abstractions.Contracts;
using Fast.Shared.Infrastructure.Modules;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Contracts;

internal sealed class ContractRegistry : IContractRegistry, IContractValidationRunner
{
    private readonly ISet<Type> _contracts = new HashSet<Type>();
    private readonly Type[] _types;
    private readonly IDictionary<string, (Type, Type)> _paths = new Dictionary<string, (Type, Type)>();
    private readonly IModuleRegistry _moduleRegistry;
    private readonly ILogger<ContractRegistry> _logger;

    public ContractRegistry(IModuleRegistry moduleRegistry, Type[] types, ILogger<ContractRegistry> logger)
    {
        _moduleRegistry = moduleRegistry;
        _types = types;
        _logger = logger;
    }

    public IContractRegistry Register<T>() where T : class, IContract
    {
        var contract = GetContractType<T>();
        _contracts.Add(contract);
        return this;
    }

    public IContractRegistry RegisterPath(string path)
        => RegisterPath<EmptyContract, EmptyContract>(path);

    public IContractRegistry RegisterPathWithRequest<TRequest>(string path) where TRequest : class, IContract
        => RegisterPath<TRequest, EmptyContract>(path);

    public IContractRegistry RegisterPathWithResponse<TResponse>(string path) where TResponse : class, IContract
        => RegisterPath<EmptyContract, TResponse>(path);
    
    public IContractRegistry RegisterPath<TRequest, TResponse>(string path)
        where TRequest : class, IContract where TResponse : class, IContract
    {
        if (path == null)
        {
            throw new ContractException("Path cannot be null.");
        }

        if (_paths.ContainsKey(path))
        {
            throw new ContractException($"Path: '{path}' is already registered.");
        }
            
        var requestContract = GetContractType<TRequest>();
        var responseContract = GetContractType<TResponse>();
        _paths.Add(path, (requestContract, responseContract));
        return this;
    }

    private static Type GetContractType<T>() where T : class => typeof(T);

    public void Validate()
    {
        if (_contracts.Any())
        {
            _logger.LogInformation($"Validating {_contracts.Count} contract(s)...");
            ValidateContracts();
        }

        if (_paths.Any())
        {
            _logger.LogInformation($"Validating {_paths.Count} path(s)...");
            ValidatePaths();
        }
    }

    private void ValidatePaths()
    {
        foreach (var (path, (requestType, responseType)) in _paths)
        {
            var registration = _moduleRegistry.GetRequestRegistration(path);
            if (registration is null)
            {
                throw new ContractException($"Request registration was not found for path: '{path}'.");
            }

            _logger.LogInformation($"Validating the contracts for path: '{path}'...");
            if (requestType != typeof(EmptyContract))
            {
                ValidateContract(requestType, path);
            }

            if (responseType != typeof(EmptyContract))
            {
                ValidateContract(responseType, path);
            }

            _logger.LogInformation($"Validated the contracts for path: '{path}'.");
        }
    }

    private void ValidateContracts()
    {
        foreach (var contractType in _contracts)
        {
            ValidateContract(contractType);
        }
    }

    private void ValidateContract(Type contractType, string? path = null)
    {
        if (Activator.CreateInstance(contractType) is not IContract contract)
        {
            return;
        }
            
        var consumer = contract.GetModuleName();
        var producer = contract.Producer;
        var contractName = contract.Type.Name;
        var originalType = _types
            .Where(x => x.FullName is not null &&
                        x.Namespace?.Contains(producer, StringComparison.InvariantCultureIgnoreCase) is true)
            .SingleOrDefault(x => x.Name == contractName);

        if (originalType is null)
        {
            throw new ContractException($"Original type was not found for contract: '{contractName}'.");
        }

        _logger.LogInformation($"Validating the contract: '{contractName}', for consumer: '{consumer}', producer: '{producer}'...");

        var originalContract = FormatterServices.GetUninitializedObject(originalType);
        var originalContractType = originalContract.GetType();
        foreach (var propertyName in contract.Properties)
        {
            var localProperty = GetProperty(contract.Type, propertyName, contractName, consumer, producer, path);
            var originalProperty = GetProperty(originalContractType, propertyName, contractName,consumer, producer, path);
            ValidateProperty(localProperty, originalProperty, propertyName, contractName, consumer, producer, path);
        }

        _logger.LogInformation($"Validated the contract: '{contractName}', for consumer: '{consumer}', producer: '{producer}'.");
    }

    private static void ValidateProperty(PropertyInfo localProperty, PropertyInfo originalProperty,
        string propertyName, string contractName, string consumer, string producer, string? path = null)
    {
        if (localProperty.PropertyType == typeof(string) && originalProperty.PropertyType == typeof(string))
        {
            return;
        }

        if (localProperty.PropertyType.IsClass && localProperty.PropertyType != typeof(string) &&
            originalProperty.PropertyType.IsClass &&
            originalProperty.PropertyType != typeof(string))
        {
            return;
        }

        if (localProperty.PropertyType == originalProperty.PropertyType)
        {
            return;
        }

        throw new ContractException($"Property: '{propertyName}' in contract: '{contractName}' (consumer: '{consumer}', producer: '{producer}')" +
                                    $"{(path is null ? "" : $", path: '{path}'")}, has a different type " +
                                    $"(actual: '{originalProperty.PropertyType}', " +
                                    $"expected: '{localProperty.PropertyType}').");
    }

    private static PropertyInfo GetProperty(Type type, string name, string contractName, string consumer,
        string producer, string? path = null)
    {
        var originalName = name;
        while (true)
        {
            var nameParts = name.Split(".");
            var property = type.GetProperty(nameParts[0]);
            if (property is null)
            {
                throw new ContractException($"Property: '{originalName}' was not found in " +
                                            $"contract: '{contractName}' (consumer: '{consumer}', producer: '{producer}')'" +
                                            $"{(path is null ? "." : $", path: '{path}'.")}");
            }

            if (property.PropertyType == typeof(string))
            {
                return property;
            }

            if (nameParts.Length == 1)
            {
                return property;
            }

            if (property.PropertyType.IsClass)
            {
                type = property.PropertyType;
                name = string.Join(".", nameParts.Skip(1));
                continue;
            }
                
            type = property.PropertyType;
            name = string.Join(".", nameParts.Skip(1));
        }
    }
        
    private class Empty
    {
    }
        
    private class EmptyContract : Contract<Empty>
    {
        public EmptyContract() : base(nameof(Empty))
        {
        }
    }

    private class ContractException : Exception
    {
        public ContractException(string message) : base(message)
        {
        }
    }
}