﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Fast.Shared.Abstractions.Contracts;
using Fast.Shared.Infrastructure.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.Shared.Infrastructure.Initializers;

internal sealed class AppInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AppInitializer> _logger;

    public AppInitializer(IServiceProvider serviceProvider, ILogger<AppInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
        
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var initializers = scope.ServiceProvider.GetServices<IInitializer>();
        var contractValidators = scope.ServiceProvider.GetServices<IContractValidator>();
        var registry = scope.ServiceProvider.GetService<IContractRegistry>();
        foreach (var initializer in initializers)
        {
            try
            {
                _logger.LogInformation($"Running the initializer: {initializer.GetType().Name}...");
                await initializer.InitAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }

        if (registry is null)
        {
            return;
        }
        
        foreach (var contractValidator in contractValidators)
        {
            try
            {
                contractValidator.Register(registry);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }
        }

        ((IContractValidationRunner) registry).Validate();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}