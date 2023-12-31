﻿using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SwizlyPeasy.Clusters.Dtos;
using SwizlyPeasy.Common.Exceptions;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Clusters.Services;

public class ServiceDiscoveryConfigProvider : IProxyConfigProvider, IHostedService, IDisposable
{
    private readonly IClusterConfigService _clusterConfigService;

    private readonly object _lockObject = new();
    private readonly IRoutesConfigService _routesConfigService;
    private readonly IOptions<SwizlyPeasyConfig> _clusterConfig;

    // To detect redundant calls
    private bool _disposedValue;
    private volatile InMemoryConfig? _inMemoryConfig;
    private Timer? _timer;

    public ServiceDiscoveryConfigProvider(IClusterConfigService clusterConfigService,
        IRoutesConfigService routesConfigService,
        IOptions<SwizlyPeasyConfig> clusterConfig)
    {
        _clusterConfigService = clusterConfigService ?? throw new ArgumentNullException(nameof(clusterConfigService));
        _routesConfigService = routesConfigService ?? throw new ArgumentNullException(nameof(routesConfigService));
        _clusterConfig = clusterConfig ?? throw new ArgumentNullException(nameof(clusterConfig));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Starting IHostedService,
    ///     A timer is created, updating the YARP configuration when the interval is reached.
    ///     -> ServiceDiscoveryConfig
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(UpdateConfig, null, TimeSpan.Zero,
            TimeSpan.FromSeconds(_clusterConfig.Value.ServiceDiscovery.RefreshIntervalInSeconds));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public IProxyConfig GetConfig()
    {
        if (_inMemoryConfig == null)
        {
            UpdateConfig(null);
        }

        if (_inMemoryConfig == null)
        {
            throw new InternalDomainException("IProxyConfig can't be null!", null);
        }

        return _inMemoryConfig;
    }

    /// <summary>
    ///     Updating the YARP configuration:
    ///     If config is null, retrieving the routes from the file routes.config.json and then saving the raw data
    ///     in consul kv store. The routes are then retrieved from kv store, allowing changes on the fly.
    /// </summary>
    /// <param name="state"></param>
    private void UpdateConfig(object? state)
    {
        lock (_lockObject)
        {
            if (_inMemoryConfig == null)
            {
                var path = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                    _clusterConfig.Value.RouteConfigFilePath);
                var currentConfig = File.ReadAllText(path);
                Debug.Assert(currentConfig != null, nameof(currentConfig) + " != null");
                _routesConfigService.LoadRoutes(_clusterConfig.Value.ServiceDiscovery.KeyValueStoreKey, currentConfig)
                    .GetAwaiter().GetResult();
            }

            var routes = _routesConfigService.GetRoutes(_clusterConfig.Value.ServiceDiscovery.KeyValueStoreKey).Result;
            var clusters = _clusterConfigService.RetrieveClustersConfig().Result;

            var oldConfig = _inMemoryConfig;
            _inMemoryConfig = new InMemoryConfig(routes, clusters);
            oldConfig?.SignalChange();
        }
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _timer?.Dispose();
        }

        _disposedValue = true;
    }
}