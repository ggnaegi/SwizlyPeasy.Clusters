﻿using SwizlyPeasy.Clusters.Providers;
using SwizlyPeasy.Clusters.Services;
using Yarp.ReverseProxy.Configuration;

namespace SwizlyPeasy.Clusters.Extensions;

public static class ReverseProxyBuilderExtensions
{
    /// <summary>
    ///     Custom configuration load mechanism.
    ///     Inspired by
    ///     https://tanzu.vmware.com/developer/blog/build-api-gateway-csharp-yarp-eureka/
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IReverseProxyBuilder LoadFromServiceDiscoveryProvider<T>(this IReverseProxyBuilder builder)
        where T : class, IServiceDiscoveryProviderClient
    {
        builder.Services.AddSingleton<IServiceDiscoveryProviderClient, T>();
        builder.Services.AddSingleton<IRetrieveDestinationsService>(ctx =>
            ctx.GetRequiredService<IServiceDiscoveryProviderClient>());
        builder.Services.AddSingleton<IHealthCheckService>(ctx =>
            ctx.GetRequiredService<IServiceDiscoveryProviderClient>());
        builder.Services.AddSingleton<IKeyValueService>(
            ctx => ctx.GetRequiredService<IServiceDiscoveryProviderClient>());


        builder.Services.AddSingleton<IClusterConfigService, ClusterConfigService>();
        builder.Services.AddSingleton<IRoutesConfigService, RoutesConfigService>();
        builder.Services.AddSingleton<ServiceDiscoveryConfigProvider>();
        builder.Services.AddSingleton<IHostedService>(ctx => ctx.GetRequiredService<ServiceDiscoveryConfigProvider>());
        builder.Services.AddSingleton<IProxyConfigProvider>(ctx =>
            ctx.GetRequiredService<ServiceDiscoveryConfigProvider>());

        return builder;
    }
}